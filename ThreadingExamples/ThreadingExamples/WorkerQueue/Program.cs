using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

namespace WorkerQueue
{
    public class BlockingQueue<T>
    {
        private readonly Queue<T> _queue = new Queue<T>();
        private bool _stopped;

        public bool Enqueue(T item)
        {
            if (_stopped)
                return false;
            lock (_queue)
            {
                if (_stopped)
                    return false;
                _queue.Enqueue(item);
                Monitor.Pulse(_queue);
            }
            return true;
        }

        public T Dequeue()
        {
            if (_stopped)
                return default(T);
            lock (_queue)
            {
                if (_stopped)
                    return default(T);
                while (_queue.Count == 0)
                {
                    Monitor.Wait(_queue);
                    if (_stopped)
                        return default(T);
                }
                return _queue.Dequeue();
            }
        }

        public void Stop()
        {
            if (_stopped)
                return;
            lock (_queue)
            {
                if (_stopped)
                    return;
                _stopped = true;
                Monitor.PulseAll(_queue);
            }
        }
    }
    class Program
    {
        static BlockingQueue<string> queue = new BlockingQueue<string>();
        static int thread_num = 1;

        static void Main(string[] args)
        {
            List<Thread> threads = new List<Thread>();

            int maxThreads = 1024;
            for( int i = 0; i < maxThreads; i++ )
            {
                Thread newThread = new Thread(WorkerMethod);
                threads.Add(newThread);
                int curr_thread_num = thread_num++;
                newThread.Start("Thread #" + curr_thread_num.ToString());  // Magic line
            }
            Console.WriteLine("Threads all running and ready to work!");
            Console.WriteLine("Press any key to start generating work");
            Console.ReadKey();

            Thread generatorThread = new Thread(GeneratorMethod);
            generatorThread.Start();

            Thread generatorThread2 = new Thread(GeneratorMethod);
            generatorThread2.Start();


            Console.WriteLine("Press any key to quit");
            Console.ReadKey();
            generatorThread.Abort();
            foreach(Thread t in threads)
            {
                t.Abort();
            }
        }

        static void GeneratorMethod()
        {
            int jobnum = 0;
            while(true)
            {
                string jobname = "Job number: " + jobnum.ToString();
                Console.WriteLine("\t\t\t\t\t\t Ready:" + jobname);
                queue.Enqueue(jobname);
                Thread.Sleep(1);
                jobnum++;  
            }
        }
        static void WorkerMethod(object name)
        {

            string myname = (string)name;
            int count = 0;
            Random rnd = new Random(Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine(" Starting a new thread! My name is: " + myname);

            //queue.Dequeue();
            while (true)
            {
                string myJob = queue.Dequeue();
                Console.WriteLine(myname + " doing job: " + myJob);
                Thread.Sleep(rnd.Next(250, 2500));
            }
        }
    }
}
