/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task4.Threads.Join
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
            Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
            Console.WriteLine("Implement all of the following options:");
            Console.WriteLine();
            Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
            Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

            Console.WriteLine();

            // feel free to add your code

            #region Thread & Join

            var thread = new Thread(() => ThreadBody(10));
            thread.Start();
            thread.Join();
            Console.WriteLine("End!");

            void ThreadBody(int value)
            {
                Console.WriteLine(value);
                int localValue = value;

                if (localValue > 0)
                {
                    localValue--;
                    var localThread = new Thread(() => ThreadBody(localValue));
                    localThread.Start();
                    localThread.Join();
                }
            }

            #endregion

            #region ThreadPool & Semaphore

            Semaphore sem = new(1, 1);

            sem.WaitOne();
            ThreadPool.QueueUserWorkItem(callBack => ThreadPoolBody(10));

            sem.WaitOne();
            Console.WriteLine("End!");
            sem.Release();

            void ThreadPoolBody(int value)
            {
                
                Console.WriteLine(value);
                int localValue = value;

                if (localValue > 0)
                {
                    localValue--;
                    var localThread = new Thread(() => ThreadPoolBody(localValue));
                    localThread.Start();
                }
                else
                {
                    sem.Release();
                }
            }

            #endregion

            Console.ReadLine();
        }
    }
}
