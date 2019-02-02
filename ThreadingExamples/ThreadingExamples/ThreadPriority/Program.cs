using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ThreadPriorityTest
{
    class Program
    {
        static void Main()

        {

            Thread thread1 = new Thread(new ThreadStart(Method1));

            Thread thread2 = new Thread(new ThreadStart(Method2));

            thread1.Priority = ThreadPriority.Highest;

            thread2.Priority = ThreadPriority.Lowest;

            thread2.Start();

            thread1.Start();

            Console.Read();

        }

        static void Method1()

        {

            for (int i = 0; i < 10; i++)

            {

                Console.WriteLine("First thread: " + i);

            }

        }

        static void Method2()

        {

            for (int i = 0; i < 10; i++)

            {

                Console.WriteLine("Second thread: " + i);

            }

        }
    }
}
