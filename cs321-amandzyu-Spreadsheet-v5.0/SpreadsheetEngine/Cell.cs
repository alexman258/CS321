using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

// Aleksandr Mandzyuk
// WSU ID - 11593044
namespace SpreadsheetEngine
{
    public abstract class Cell : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;   // Property changed event handler
        protected int rowIndex;       // Private rowIndex property
        protected int columnIndex;   // Private columnIndex property
        protected string text;        // Text inside the cell that will be displayed
        protected string value;


        public Cell() {}
        public Cell(int colIndex, int rIndex)      // Constructor. Sets rowIndex and ColumnIndex.
        {
            rowIndex = rIndex;
            columnIndex = colIndex;

        }

       

        public int RowIndex         // Property for getting rowIndex.
        {
            get {return rowIndex;}
        }

        public int ColumnIndex     // Property for getting columnIndex.
        {
            get{ return columnIndex; }
        }

        public string Text          // Property for either setting or getting string text.
        {
            get { return text; }
            
            set                     // Sets text. If value is identical to text then exit out of setter. Otherwise, set value.
            {

                if (value != text)
                {
                    text = value;
                  
                    OnPropertyChanged("Text");  //PropertyChanged event is called whenever Text is updated
                }
                                   
            }
        }

        public string Value
        {
            get{ return value; } 
        }

        // Create CellPropertyChanged method to raise the event
        protected void OnPropertyChanged(string text)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(text));
            }

        } 
    }
}
