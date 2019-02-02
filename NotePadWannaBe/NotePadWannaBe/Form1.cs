// Aleksandr Mandzyuk 
// ID - 11593044
// Homework 3 - Winforms Notepad Application

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.IO;
using System.Numerics;


namespace NotePadWannaBe
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();        
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Don't delete.
        }


        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Don't delete.
        }

        private void loadFromFileToolStripMenuItem_Click(object sender, EventArgs e) // Load From File function
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();              // New openFileDialog object is created

            openFileDialog1.Filter = "Text Files (.txt) |*.txt|All Files (*.*)|*.*";    //Filters what kind of files can be opened
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            openFileDialog1.ShowDialog();                   // Display dialog
            
            Stream fileStream = openFileDialog1.OpenFile();       // Create new stream

            StreamReader reader = new StreamReader(fileStream);     // Create streamreader and pass it on to my own 
            LoadText(reader);                                       // LoadText function to open up a file.
  
            
        }

        private void saveToFileToolStripMenuItem_Click(object sender, EventArgs e)      // Save to file function
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();      // Uses SaveFileDialog Class

            saveFileDialog1.Filter = "Text Files (.txt) |*.txt";        // Can only be saved as .txt files
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            StringBuilder sBuilder = new StringBuilder();       
          
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sBuilder.Append(textBox1.Text);                  // Uses stringbuilder to copy contents in application to stringbuilder
                TextWriter textWriter = new StreamWriter(saveFileDialog1.FileName);   // Uses textwriter to write stringbuilder to file
                textWriter.Write(sBuilder.ToString());
                textWriter.Flush();                 // Flush and close textwriter
                textWriter.Close();
            }

        }

        private void LoadText(TextReader sr)    // Load text function. Receives TextReader as parameter
        {

            if (sr != null)         // Checks if empty
            {
                textBox1.Text = "";     // Empties text box
                string output = sr.ReadToEnd();     //Creates a new string that contains everything passed in
                textBox1.Text = output;             // Outputs this string to to notepad application
                sr.Close();
            }
                

        }

        public class FibonacciTextReader : TextReader  // FibTextReader class. Inherits from TextReader Class
        {
            private int numsToGen;                  // Amount of numbers to generate

            public FibonacciTextReader(int numsToGenerate)      // Constructor. 
            {
                numsToGen = numsToGenerate;                 
            }

            int count = 0;              // Count variable.
            BigInteger first;           // Holds first and second values for fibonacci generator
            BigInteger second;

            StringBuilder sBuilder = new StringBuilder();       //Builds string of fibonacci sequence


            public override string ReadLine()  // Overridden read line function
            {
                BigInteger temp;            // Temporary big integer placeholder
               if (count == 0)
                {
                    first = 0;
                    count++;
                    return first.ToString();
                }
               else if (count == 1)         // If count = 1 or 0, then return 0 1 
                {
                    second = 1;
                    count++;
                    return second.ToString();
                }
                else                        // If count >= 2, generate next fibonacci number
                {
                    temp = second;
                    second = first + second;
                    first = temp;
                    count++;
                    return second.ToString();
                }
            }

            public override string ReadToEnd()      // Overridden ReadToEnd
            {
                while (count < numsToGen)       // While count is less than amount of numbers to generate
                {                               // append next number to stringbuilder.
                    sBuilder.Append(count + 1).Append(": ").Append(ReadLine()).AppendLine();
                }
                return sBuilder.ToString();     // Return stringbuilder containing fibonacci sequence
            }

        }

        private void first50NumbersToolStripMenuItem_Click(object sender, EventArgs e) // First 50 fib's function
        {
            int fibNum = 50;                // Number of fibonacci's to generate
            FibonacciTextReader fibFifty = new FibonacciTextReader(fibNum); // Create new instance of Fibonacci Text Reader
            LoadText(fibFifty);                 // Send object to LoadFile function
        }

        private void first100NumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int fibNum = 100;               // Number of fibonacci's to generate
            FibonacciTextReader fibHundred = new FibonacciTextReader(fibNum);// Create new instance of Fibonacci Text Reader        
            LoadText(fibHundred);   // Send object to LoadFile function
        }
    }
}
