using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

// Aleksandr Mandzyuk
namespace SpreadsheetEngine
{   
    // SpreadsheetCell class that inherits from Cell and can be implemented
    class SpreadsheetCell : Cell
    {
        // Constructor. Sets rowIndex and ColumnIndex.
        public SpreadsheetCell(int colIndex, int rIndex)   
        {
            rowIndex = rIndex;
            columnIndex = colIndex;
        }

        public SpreadsheetCell()
        {

        }

        public void SetValue(string text)       //Set value of cell
        {
            value = text;
        }
    }

    // Spreadsheet Class
    // Has a 2-d array of Spreadsheet cells
    public class Spreadsheet
    {

        // Name of the Application along with the current version number.
        public static string Name { get { return "CoolSheet"; } }
        private static Version version = new Version(5, 0);
        public static Version Version { get { return version; } }
        
        // Dictionary that holds all the interdependencies of the cells
        // The key is a certain cell. And the value is the list of all the cells that depend on the key cell
        Dictionary<Cell, List<Cell>> Dependencies = new Dictionary<Cell, List<Cell>>();

        // Dictionary that holds all the variables
        // Key is the name of a cell. Value is the actual value of that cell
        Dictionary<string, double> VarTable = new Dictionary<string, double>();
    
        protected int RowCount { get; }      // Amount of rows and columns in the spreadsheet.
        protected int ColumnCount { get; }

        public event PropertyChangedEventHandler CellPropertyChanged;

        Cell[,] cells;


        // Spreadsheet Constructor
        // Creates a 2-d array of cells and subscribes the SheetPropertyChanged Event
        // To the Property changed event of each cell
        public Spreadsheet(int columns, int rows)       
        {
            RowCount = rows;
            ColumnCount = columns;
            cells = new SpreadsheetCell[columns, rows];
            for(int i = 0; i < columns; i++)
            {
                for(int j = 0; j < rows; j++)
                {
                    cells[i, j] = new SpreadsheetCell(i, j);
                    cells[i, j].PropertyChanged += SheetPropertyChanged;
                }
            }

        }

        // Returns Cell at a certain index
        public Cell GetCell(int column, int row)
        {
            return cells[column, row];
        }

        // Takes an input string converts it to coordinates. Calls other GetCell function
        public Cell GetCell(string coordinates)
        {
            if(coordinates[0] < 'A' || coordinates[0] > 'Z')
            {
                throw new System.ArgumentException("Column Index out of range");
            }
            int column = coordinates[0] - 'A';
            int row = Convert.ToInt32(coordinates.Substring(1)) - 1;
            return GetCell(column, row);
        }

        // Returns the value of a cell based on an input string. Ex: "=B3" returns value at cell B3
        private string CalculateVal(string text)
        {
            if(text[0] != '=')      // It not starting with '=' then the value is returned
            {
                return text;
            }
            string calculate = text.Substring(1); // Gets rid of =
            return GetCell(calculate).Value;
        }

        // Create SheetPropertyChanged method to raise event
        //
        private void SheetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ExpTree tree = new ExpTree(VarTable);
            var currCell = (SpreadsheetCell)sender;

            // Check if cell is a valid input. Will get finish in the future
            if(!IsValid(currCell, Dependencies))
            {
                currCell.SetValue("#REF");
                return;
            }

            if (currCell.Text != null && currCell.Text[0] == '=')
            {
                // Create a new expression tree if the cell text starts with =
                tree.SetExp(currCell.Text.Substring(1));

                // Set the cell value to the evaluation of the expression
                currCell.SetValue(tree.Eval().ToString());

                // If the list of variables inside this cell is not null then
                // Add each variable to the dependencies dictionary and
                // set the current cell as being dependent to that cell
                if(tree.VarsForDeps != null)
                {
                    foreach(string s in tree.VarsForDeps)
                    {
                        if(!Dependencies.ContainsKey(GetCell(s)))
                        {
                            Dependencies.Add((GetCell(s)), new List<Cell>());
                        }
                        Dependencies[GetCell(s)].Add(currCell);
                    }
                }
            }
            else
            {
                // If currCell doesn't start with '=', we set the value of the cell to the text that was entered
                currCell.SetValue(currCell.Text);

                // If variable is not currently in the variable table then
                // We add the cell name and its contents to the variable table
                string temp = CoordToName(currCell.ColumnIndex, currCell.RowIndex);
                if (!(VarTable.ContainsKey(temp)))
                {
                    VarTable.Add(temp, Convert.ToDouble(currCell.Value));
                }

                
                // If a cell got its value changed to a number, then it is no longer dependent on other cells
                // The foreach loops go through each cell in the dependencies dictionary
                // And make sure that the cell is no longer being written as being dependent
                // On another cell
                Dictionary<Cell, List<Cell>> NewDeps = new Dictionary<Cell, List<Cell>>();
                foreach (KeyValuePair<Cell, List<Cell>> cells in Dependencies)
                {
                    foreach(Cell cell in cells.Value)
                    {
                       if(currCell != cell)
                       {
                            if(!NewDeps.ContainsKey(cells.Key))
                            {
                                NewDeps.Add(cells.Key, new List<Cell>());
                            }
                            NewDeps[cells.Key].Add(cell);
                       }
                    }
                }
                // Update the dependencies dictionary.
                Dependencies = NewDeps;
            }

            // Update variables, dependencies, and call CellPropertyChanged.
            UpdateVars(currCell);
            UpdateDependencies(currCell);
            CellPropertyChanged(sender, e);
            
        }

        // Function to update the Dependencies
        private void UpdateDependencies(SpreadsheetCell cell)
        {
            // Create a new tree with current variables
            ExpTree tree = new ExpTree(VarTable);

            
            if (Dependencies.ContainsKey(cell))
            {
                // For each of the cells dependent on cell, update dependencies
                foreach(SpreadsheetCell myCell in Dependencies[cell])
                {
                    tree.SetExp (myCell.Text.Substring(1));
                    myCell.SetValue(tree.Eval().ToString());
                    CellPropertyChanged(myCell, new PropertyChangedEventArgs("Value"));
                    UpdateVars(myCell);
                    UpdateDependencies(myCell);
                }
            }
        }


        // Returns the name of a cell based on its coordinates.
        // CoordToName(3,3) would return C3
        private string CoordToName(int column, int row)
        {
            string name = ((char)(column + 'A')).ToString();
            name += (row + 1).ToString();
            return name;
        }

        // Updates the table of variables
        private void UpdateVars(Cell cell)
        {
            string cellName = CoordToName(cell.ColumnIndex, cell.RowIndex);
            if(!VarTable.ContainsKey(cellName))
            {
                VarTable.Add(cellName, Convert.ToDouble(cell.Value));
            }
            VarTable[cellName] = Convert.ToDouble(cell.Value);
        }

        //Check if the cell input is valid
        // NOT DONE. NEEDS WORK
        private bool IsValid(Cell cell, Dictionary<Cell,List<Cell>> Deps)
        {
            if(Deps.ContainsKey(cell))
            {
                if(Deps[cell].Contains(cell))
                {
                    return false;
                }
            }
            return true;
        }

        // Function that returns an array of non empty cells.
        // Used to find out which cells to save to XML.
        private List<Cell> ReturnCells()
        {
            List<Cell> cellsToReturn = new List<Cell>();

            for(int i = 0; i < ColumnCount; i++)
            {
                for(int j = 0; j < RowCount; j++)
                {
                    if(cells[i,j].Text != null)
                    {
                        cellsToReturn.Add(cells[i, j]);
                    }
                }
            }

            return cellsToReturn;
        }

        // Takes a stream of the file path and saves to that file.
        // Saves all cells that aren't empty into an XML document.
        public void SaveToXml(StreamWriter myStream)
        {
            XDocument SpreadsheetDoc = new XDocument(
                new XComment("Saving all non empty cells into XML format"),
                
                new XElement("SpreadsheetCells",

                    from cell in ReturnCells()
                    select new XElement("Cell",
                            new XElement ("Index", CoordToName(cell.ColumnIndex, cell.RowIndex)),
                            new XElement("Text", cell.Text),
                            new XElement("Value", cell.Value))
                ));

            SpreadsheetDoc.Save(myStream);

        }

        // Load XML file
        // First, clears out the array inside spreadsheet class. Next,
        // Goes through every cell in XML file and inputs into spreadsheet class.
        public void LoadFromXML(StreamReader myStream)
        {
            var spreadsheetDoc = XDocument.Load(myStream);
                foreach (XElement x in spreadsheetDoc.Descendants("Cell"))
                {
                //Temp Variables to hold index, text, and value
                string cellName = null;
                string text = null;
                string val = null;
                    foreach(XElement y in x.Descendants())
                    {
                        if(y.Name == "Index")
                        {
                        cellName = y.Value;
                            //CurCell = (SpreadsheetCell)GetCell(y.Value);
                        }
                        else if(y.Name == "Text")
                        {
                        text = y.Value;
                        }
                        else if(y.Name == "Value")
                        {
                        val = y.Value;
                        }
                        //Set cell at a certain index equal to values correlated to that index.
                        if (cellName != null)
                        {
                            ((SpreadsheetCell)GetCell(cellName)).Text = text;
                            ((SpreadsheetCell)GetCell(cellName)).SetValue(val);
                        } 
                    }
                }
        }

        // Clears out entire spreadsheet
        public void ClearSheet()
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                for (int j = 0; j < RowCount; j++)
                {
                    SpreadsheetCell temp = (SpreadsheetCell)cells[i, j];
                    temp.Text = null;
                    temp.SetValue(null);  
                }
            }
        }
    }
}
