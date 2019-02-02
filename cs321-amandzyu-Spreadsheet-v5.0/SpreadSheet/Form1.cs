using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpreadsheetEngine;
using System.IO;

// Aleksandr Mandzyuk
// WSU ID - 11593044
namespace Spreadsheet
{
    public partial class Spreadsheet : Form
    {
        // Instance of a new Spreadsheet object
        private SpreadsheetEngine.Spreadsheet SpSheet;
        //public string version = SpreadsheetEngine.Spreadsheet.Version;

        public Spreadsheet()
        {
            
            InitializeComponent();
            SpSheet = new SpreadsheetEngine.Spreadsheet(26, 50);    // Set size of spreadsheet
            SpSheet.CellPropertyChanged += Spreadsheet_CellPropertyChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Creating columns A through Z in DataGridView
            char X = 'A';
            for (int i = 0; i < 26; i++)
            {
                dataGridView1.Columns.Add(i.ToString(), X.ToString());
                X++;
            }

            // Creatins rows 1-50 in DataGridView
            for (int i = 1; i < 51; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i - 1].HeaderCell.Value = i.ToString();
            }
            textBox1.Focus();
        }

        // Function that updates GUI to backend
        private void Spreadsheet_CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
           var curCell = (Cell)sender;

            switch(e.PropertyName)
            {
                case "Value":
                    dataGridView1[curCell.ColumnIndex, curCell.RowIndex].Value = curCell.Value;
                    break;
                case "Text":
                    dataGridView1[curCell.ColumnIndex, curCell.RowIndex].Value = curCell.Text;
                    break;
                default:
                    break;
            }
  
        }

        

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox1.Focus();
        }

        private void dataGridView1_CellBeginEdit_1(object sender, DataGridViewCellCancelEventArgs e)
        {
            dataGridView1[e.ColumnIndex, e.RowIndex].Value = SpSheet.GetCell(e.ColumnIndex, e.RowIndex).Text;
        }

        private void dataGridView1_CellEndEdit_1(object sender, DataGridViewCellEventArgs e)
        {
            var val = dataGridView1[e.ColumnIndex, e.RowIndex].Value;
            if (val != null)
            {
                Cell cell = SpSheet.GetCell(e.ColumnIndex, e.RowIndex);
                cell.Text = val.ToString();
                dataGridView1[e.ColumnIndex, e.RowIndex].Value = cell.Value;
            }
        }

        // When enter is pressed in the text box, update the cell with text box text
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            { 
                int cellCol = dataGridView1.CurrentCell.ColumnIndex;
                int cellRow = dataGridView1.CurrentCell.RowIndex;
                SpSheet.GetCell(cellCol, cellRow).Text = textBox1.Text;
                dataGridView1[cellCol, cellRow].Value = SpSheet.GetCell(cellCol, cellRow).Value;
                textBox1.Clear();

            }
        }

        // When a cell is clicked, direct mouse to the textbox
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox1.Clear();
            if (dataGridView1[e.ColumnIndex, e.RowIndex].Value != null)
            {
                textBox1.Text = SpSheet.GetCell(e.ColumnIndex, e.RowIndex).Text;
            }
            textBox1.Focus();
        }


        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.Show();
        }

        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text Files (.xml) |*.xml";        // Can only be saved as .txt files
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                {
                    SpSheet.SaveToXml(sw);
                }
            }

        }

        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "Text Files (.xml) |*.xml";    //Filters what kind of files can be opened
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Stream filestream = openFileDialog1.OpenFile();
                using (StreamReader sr = new StreamReader(openFileDialog1.OpenFile()))
                {
                    SpSheet.ClearSheet();
                    SpSheet.LoadFromXML(sr);
                }
            } 





        }
    }
}
