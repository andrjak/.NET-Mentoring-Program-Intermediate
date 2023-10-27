/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreading.Task2.Chaining
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(".Net Mentoring Program. MultiThreading V1 ");
            Console.WriteLine("2.	Write a program, which creates a chain of four Tasks.");
            Console.WriteLine("First Task – creates an array of 10 random integer.");
            Console.WriteLine("Second Task – multiplies this array with another random integer.");
            Console.WriteLine("Third Task – sorts this array by ascending.");
            Console.WriteLine("Fourth Task – calculates the average value. All this tasks should print the values to console");
            Console.WriteLine();

            var chain = Task.Run(() =>
            {
                const int size = 10;
                var random = new Random();

                int[] array = new int[size];

                for (int i = 0; i < size; i++)
                {
                    array[i] = random.Next(10);
                }

                ArrayPrint(array);
                
                return array;
            }).ContinueWith(previousArrayTask =>
            {
                var array = previousArrayTask.Result;
                var random = new Random();
                var randomValue = random.Next(10);

                for (int i = 0; i < array.Length; i++)
                {
                    array[i] *= randomValue;
                }

                ArrayPrint(array);

                return array;
            }).ContinueWith(previousArrayTask =>
            {
                var array = previousArrayTask.Result;
                Array.Sort(array);

                ArrayPrint(array);

                return array;
            }).ContinueWith(previousArrayTask =>
            {
                var array = previousArrayTask.Result;
                var result = array.Average(x => x);

                Console.WriteLine(result);
                return result;
            });

            chain.Wait();

            Console.ReadLine();

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
        }
    }
}
