using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    // ExpTree Class
    public class ExpTree
    {

        public Node root = null;           
        private string expString;
        public Dictionary<string, double> vars;
        private char[] ops = { '+', '-', '*', '/' };

        // List of all variables in an expression.
        // This is a list of all the Cells that our current cell depends on.
        // So if A2's text is "=A1+C3*B5", the list would be: A1,C3,B5
        // In our Dependencies dictionary, we can set each of these cell's dependencies to A2
        private List<string> varsForDeps = new List<string>();

        public List<string> VarsForDeps
        {
            get { return varsForDeps; }
        }

        public ExpTree(string expression)       // Expression tree constructor
        {
            expString = expression;
            vars = new Dictionary<string, double>();
           // BuildTree(expString);
        }

        public ExpTree(Dictionary<string, double> varTable)
        {
            vars = varTable;
        }

        public void SetVar(string varName, double varValue)     // Function for setting variable and strong variable and name inside dictionary
        {
            vars[varName] = varValue;
        }

        public void SetExp(string exp)
        {
            expString = exp;
        }

        public double Eval()            // Public Evaluation functions. Calls private eval and passes in root.
         {
            BuildTree(expString);
            return Eval(root);
        }

        private double Eval(Node node)  // Evaluation function that gets called by the public eval.
        {
            // If the node is a value node, return the value stored in that node.
            if (node is ValNode)
            {
                return (node as ValNode).getValue();
            }
            
            // If the node is a variable node, return the value that is associated with that variable in the dictionary
            if (node is VarNode)
            {
                //var temp1 = node as VarNode;
                return vars[(node as VarNode).getName()];
            }

            // If the node is an operator node. Do the operation
            //var temp = node as OpNode;

            switch ((node as OpNode).getOp())
            {
                case '*':
                    return Eval((node as OpNode).leftChild) * Eval((node as OpNode).rightChild);

                case '/':
                    return Eval((node as OpNode).leftChild) / Eval((node as OpNode).rightChild);

                case '+':
                    return Eval((node as OpNode).leftChild) + Eval((node as OpNode).rightChild);

                case '-':
                    return Eval((node as OpNode).leftChild) - Eval((node as OpNode).rightChild);
            }

            return -1;
        }

        public abstract class Node {     }  // Parent node that all children nodes inherit from

        private class OpNode : Node         // Operator node
        {
            char op;
            public Node leftChild, rightChild;
            public OpNode(char oper)
            {
                op = oper;
            }

            public char getOp()
            {
                return op;
            }
        }

        private class ValNode : Node        // Node containing constant numerical value
        {
            double val;
            public ValNode(double value)
            {
                val = value;
            }
            public double getValue()
            {
                return val;
            }
        }

        private class VarNode : Node        // Node containing a variable
        {
            string var;
            public VarNode(string variable)
            {
                var = variable;
            }
            public string getName()
            {
                return var;
            }
        }


        // Build tree function. Takes in expression and separates operators from values and variables
        private void BuildTree(string exp)
        {
            // Use Regex Split to go through expression string and tokenize the string into substrings
            // Each substring that gets split is put into a List<string>
            // Each substring is made sure to be not empty
            // Regex Expression: ([+-/*\(\)])   :   Any operator or parentheses
            // Any variable or number will be separated by operators or parentheses.
            // So this makes sense to use the operators to do the splitting
            List<string> tokens = new List<string>();
            tokens = Regex.Split(exp, @"([+-/*\(\)])").Where(s => s != String.Empty).ToList<string>();
            // op       [+\\-/*()\\^]
            // digit    [\\d]+
            // variable [a-zA-Z]+;


            // Adds all the variables to a list to be used for our dictionary of dependencies
            foreach(string s in tokens)
            {
                if(Char.IsUpper(s[0]))
                {
                    varsForDeps.Add(s);
                }
            }

            // Convert from infix notation to reverse polish
            tokens = ToPolish(tokens);
            // Using a stack structure to create our tree
            // Operands are pushed onto the stack
            // Operators pop the necessary operands off the stack and set those as its children
            // This operator node then gets pushed onto the stack
            Stack<Node> stack = new Stack<Node>();

            foreach(string s in tokens)
            {
                // Creates variable node. Sets default value to zero
                if(Char.IsLetter(s,0))
                {
                    stack.Push(new VarNode(s));
                    if(!(vars.ContainsKey(s)))
                    {
                        SetVar(s, 0);
                    }
                }
                else if (Char.IsDigit(s,0))
                {
                    stack.Push(new ValNode(Convert.ToDouble(s)));
                }
                else
                {
                    OpNode temp = new OpNode(s[0]);
                    temp.rightChild = stack.Pop();
                    temp.leftChild = stack.Pop();
                    stack.Push(temp);
                }

            }
            root = stack.Pop();            
         }

        
        // Function that takes an expression in Infix notation and converts to Reverse Polish Notation
        private List<string> ToPolish(List<string> str)
        {
            Stack<string> opStack = new Stack<string>();
            List<string> rpn = new List<string>();

            foreach(string s in str)
            {
                switch(s)
                {
                    case "+": case "-": case "*": case "/":     // Case for operators
                        if(opStack.Count() > 0)
                        {
                            while(opStack.Count > 0 && Precedence(opStack.Peek()) > Precedence(s)) // If top stack operator has greater precedence than current item
                            {                                                 // Pop off stack and add to output list
                                rpn.Add(opStack.Pop());
                            }
                        }
                        opStack.Push(s);
                        break;
                    case "(":
                        opStack.Push(s);
                        break;
                    case ")":
                        while (opStack.Peek() != "(")
                        {
                            rpn.Add(opStack.Pop());
                        }
                        opStack.Pop();
                        break;
                    default:
                        rpn.Add(s);
                        break;
                }
            }
            while(opStack.Count() > 0)
            {
                rpn.Add(opStack.Pop());
            }
            return rpn;
        }

        // Function to set the precedence of operators. Used for shunting yard algorithm
        private int Precedence(string s)
        {
            switch(s)
            {
                case "(": case ")":
                    return 0;
                case "+": case "-":
                    return 1;
                case "*": case "/":
                    return 2;      
                default:
                    return -1;
            }
        }
    }
}
