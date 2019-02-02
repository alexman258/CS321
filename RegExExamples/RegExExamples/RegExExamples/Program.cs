using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace RegExExamples
{
    class Program
    {

        static void simpleExample()
        {

            // Lets use a regular expression to match a date string.
            string pattern = @"([a-zA-Z]+) (\d+)";

            // The RegexOptions are optional to this call, we will go into more detail about
            // them below.
            Match result = Regex.Match("June 24    October 22", pattern);
            if (result.Success)
            {
                // Indeed, the expression "([a-zA-Z]+) (\d+)" matches the date string

                // To get the indices of the match, you can read the Match object's
                // Index and Length values.
                // This will print [0, 7], since it matches at the beginning and end of the 
                // string
                Console.WriteLine("Match at index [{0}, {1}]",
                    result.Index,
                    result.Index + result.Length);

                // To get the fully matched text, you can read the Match object's Value
                // This will print "June 24"
                Console.WriteLine("Match: {0}", result.Value);

                // If you want to iterate over each of the matches, you can call the 
                // Match object's NextMatch() method which will return the next Match
                // object.
                // This will print out each of the matches sequentially.
                while (result.Success)
                {
                    Console.WriteLine("Match: {0}", result.Value);
                    result = result.NextMatch();
                }
            }
        }

        static void collectionExample()
        {
            // Lets use a regular expression to capture data from a few date strings.
            string pattern = @"([a-zA-Z]+) (\d+)";
            MatchCollection matches = Regex.Matches("June 24, August 9, Dec 12", pattern);

            // This will print the number of matches
            Console.WriteLine("{0} matches", matches.Count);

            // This will print each of the matches and the index in the input string
            // where the match was found:
            //   June 24 at index [0, 7)
            //   August 9 at index [9, 17)
            //   Dec 12 at index [19, 25)
            foreach (Match match in matches)
            {
                Console.WriteLine("Match: {0} at index [{1}, {2}]",
                    match.Value,
                    match.Index,
                    match.Index + match.Length);
            }

            // For each match, we can extract the captured information by reading the 
            // captured groups.
            foreach (Match match in matches)
            {
                GroupCollection data = match.Groups;
                // This will print the number of captured groups in this match
                Console.WriteLine("{0} groups captured in {1}", data.Count, match.Value);

                // This will print the month and day of each match.  Remember that the
                // first group is always the whole matched text, so the month starts at
                // index 1 instead.
                Console.WriteLine("Month: " + data[1] + ", Day: " + data[2]);

                // Each Group in the collection also has an Index and Length member,
                // which stores where in the input string that the group was found.
                Console.WriteLine("Month found at[{0}, {1}]",
                    data[1].Index,
                    data[1].Index + data[1].Length);
            }
        }

        static void searchAndReplace()
        {
            // Lets try and reverse the order of the day and month in a few date 
            // strings. Notice how the replacement string also contains metacharacters
            // (the back references to the captured groups) so we use a verbatim 
            // string for that as well.
            string pattern = @"([a-zA-Z]+) (\d+)";
            string startString = "June 24, August 9, Dec 12";
            Console.WriteLine("Starting string: " + startString);

            // This will reorder the string inline and print:
            //   24 of June, 9 of August, 12 of Dec
            // Remember that the first group is always the full matched text, so the 
            // month and day indices start from 1 instead of zero.
            string replacedString = Regex.Replace("June 24, August 9, Dec 12",
                pattern, @"$2 of $1");
            Console.WriteLine("  Ending string: " + replacedString);
        }

        static void simpleMatch(string pattern, string inString)
        {
            Match result = Regex.Match(inString, pattern);
            if (result.Success)
            {
                while (result.Success)
                {
                    Console.WriteLine("Match: {0}", result.Value);
                    result = result.NextMatch();
                }
            }
            else
            {
                Console.WriteLine(" [x] No regex match with pattern: {0} on string: {1}", pattern, inString);
            }
        }

        static void Main(string[] args)
        {
            simpleExample();
            Console.WriteLine("------------------------------------------------------");
            collectionExample();

            Console.WriteLine("------------------------------------------------------");
            searchAndReplace();

            Console.WriteLine("------------------------------------------------------");
            simpleMatch("hello", "hello world");
            simpleMatch(@"^[a-zA-Z]+\d+$", "A4String");
            Console.WriteLine("All done. ");
            Console.ReadKey();
        }
    }
}
