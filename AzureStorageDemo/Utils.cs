using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorageDemo
{
    public static class Utils
    {
        public static void Time(string description, Action action)
        {
            Console.WriteLine($"Starting : {description}");
            var sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            Console.WriteLine($"Completed in {sw.ElapsedMilliseconds} milliseconds");
        }

        public static async Task Time(string description, Func<Task> task)
        {
            Console.WriteLine($"Starting : {description}");
            var sw = new Stopwatch();
            sw.Start();
            await task();
            sw.Stop();
            Console.WriteLine($"Completed in {sw.ElapsedMilliseconds} milliseconds");
        }

        public static void WriteError(string error)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ResetColor();
        }

        private static readonly Random Random = new Random();
        /// <summary>
        /// NB : This isn't thread safe unless caller proves random
        /// </summary>
        /// <param name="length"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static string RandomString(int length, Random random = default)
        {
            var randomToUse = random ?? Random;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[randomToUse.Next(s.Length)])
                .ToArray());
        }

    }
}
