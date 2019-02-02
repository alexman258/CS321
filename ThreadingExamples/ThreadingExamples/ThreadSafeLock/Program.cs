using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ThreadSafeLock
{
    class ThreadSafe
    {
        static readonly object _locker = new object();
        static int _val1, _val2;

        static void Go()
        {
            Console.WriteLine("Starting ThreadSafe Go");
            lock (_locker)
            {
                Console.WriteLine("I got the lock!");
                if (_val2 != 0) Console.WriteLine(_val1 / _val2);
                _val2 = 0;
                Console.WriteLine("I am releaseing the lock");
            }
            Console.WriteLine("Lock is released");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Thread t1 = new Thread(NewThread);
            Thread t2 = new Thread(NewThread);

            t1.Start("Thread 1");
            t2.Start("Thread 2");

            t1.IsBackground = true;     // No longer need to abort if the Main() thread ends!
            t2.IsBackground = true;

            Console.WriteLine("Main Thread done.");
            Console.ReadKey();
            t1.Abort();
            t2.Abort();
        }
        static void NewThread(object name)
        {
  
            string myname = (string)name;
            int count = 0;
            Random rnd = new Random(Thread.CurrentThread.ManagedThreadId);

            Console.WriteLine(" Starting a new thread! My name is: " + myname);
            while (true)
            {
                Console.WriteLine(myname + " count: " + count++);
                Thread.Sleep(rnd.Next(250, 2500));
            }
        }
    }
}
