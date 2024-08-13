/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();

            // feel free to add your code

            AutoResetEvent canWrite = new(true);
            AutoResetEvent canRead = new(false);
            List<int> sharedList = new();

            Task.Run(() =>
            {
                canWrite.WaitOne();
                for (int i = 0; i < 10; i++)
                {
                    sharedList.Add(i);
                    canRead.Set();
                    canWrite.WaitOne();
                }
            });

            Task.Run(() =>
            {
                canRead.WaitOne();
                for (int i = 0; i < 10; i++)
                {
                    ArrayPrint(sharedList);
                    canWrite.Set();
                    canRead.WaitOne();
                }
            });

            static void ArrayPrint(IEnumerable<int> array)
            {
                StringBuilder sb = new();
                sb.Append("[ ");
                foreach (var item in array)
                {
                    sb.Append(item);
                    sb.Append(", ");
                }

                sb.Remove(sb.Length - 2, 2);
                sb.Append(" ]");
                Console.WriteLine(sb.ToString());
            }

            Console.ReadLine();
        }
    }
}
