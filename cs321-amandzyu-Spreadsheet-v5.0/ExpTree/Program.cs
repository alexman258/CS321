using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpTree
{
    class Program
    {
       
        static SpreadsheetEngine.ExpTree defaultTree = new SpreadsheetEngine.ExpTree("");
        static void Main(string[] args)
        {

            while(true)
            {
             Menu(PrintOptions());
            }
        

        }

        public static char PrintOptions()
        {
            Console.WriteLine("Menu");
            Console.WriteLine(" 1 = Enter a new expression");
            Console.WriteLine(" 2 = Set a variable value");
            Console.WriteLine(" 3 = Evaluate tree");
            Console.WriteLine(" 4 = Quit");

            return Console.ReadKey().KeyChar;
        }

        public static void Menu(char x)
        {
            switch (x)
            {
                case '1':
                    //Console.WriteLine();
                    Console.WriteLine("\nEnter your expression: ");
                    string exp = Console.ReadLine().ToString();
                    defaultTree.SetExp(exp);

                    break;

                case '2':
                    
                    Console.WriteLine("\nWhat is your variable name?");
                    string varName = Console.ReadLine().ToString();

                    Console.WriteLine();
                    Console.WriteLine("What is the value of " + varName + " ?");
                    string varValue = Console.ReadLine().ToString();

                    defaultTree.SetVar(varName, Convert.ToDouble(varValue));
                    break;

                case '3':
                    Console.WriteLine();
                    Console.WriteLine(defaultTree.Eval().ToString());
                    break;

                case '4':
                    Environment.Exit(0);
                    break;

                default:
                    Environment.Exit(0);
                    break;
            }
        }
    }
}
