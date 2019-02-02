using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ThreadMaxCountTest
{
    class Program
    {
        static void DummyCall()
        {
            Thread.Sleep(1000000000);
        }
        static void Main(string[] args)
        {
            int count = 0;
            var threadList = new List<Thread>();
            try
            {
                while (true)
                {
                    Thread newThread = new Thread(new ThreadStart(DummyCall), 1024);
                    threadList.Add(newThread);
                    newThread.Start();  // Magic line
                    count++;
                    Console.WriteLine("Thread count: " + count);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
