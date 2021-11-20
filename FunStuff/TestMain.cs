using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunStuff
{
    public class TestMain
    {
        public static void Main(string[] args)
        {
            //TestFactorial();
            TestWhatChanged();
        }

        static void TestWhatChanged()
        {
            char[] original = { 'A', 'B', 'C', 'D' };
            char[] changed = { '1', 'A', 'C', '3', '4' };

            var result = WhatChanged.GetChanges(original, changed);

            for(int x = 0; x < result.Count; x++) {
                if (original.Length > x)
                    Console.Write($"({original[x]}),");
                else
                    Console.Write("( ),");
                Console.Write($"({result[x].id},{result[x].change})");

                if (changed.Length > x)
                    Console.Write($",({changed[x]})");
                else
                    Console.Write(",( )");

                Console.WriteLine();
            }

        }
        static void TestFactorial()
        {
            bool useThreading = false;
            bool compare = false;
            Console.WriteLine("Use all processors (0), Use Single Processor(1), Compare both(2)");
            string answer = Console.ReadLine();
            if (answer == "1")
            {
                Console.WriteLine("Using a single processor (for large numbers will be slower");

            }
            else if (answer == "0")
            {
                useThreading = true;
                Console.WriteLine($"Using {Environment.ProcessorCount} processors");
            }
            else
            {
                compare = true;
                Console.WriteLine($"Comparing times for single vs {Environment.ProcessorCount} processors");
            }
            while (true)
            {


                //we can't use a long for calculating
                //a long can only hold 9,223,372,036,854,775,807
                //this is easily surpassed after a factorial of only 20! (wow amazing!)
                Console.WriteLine("Enter Number: ");
                var number = int.Parse(Console.ReadLine());

                var startTime = DateTime.Now;
                string result = "";

                if (compare)
                {
                    result = FactorialCalculator.SingleProcessor(number);
                    FactorialCalculator.PrintResults(result, "Single Processor", DateTime.Now - startTime);
                    startTime = DateTime.Now;
                    result = FactorialCalculator.Threading(number, Environment.ProcessorCount);
                    FactorialCalculator.PrintResults(result, $"{Environment.ProcessorCount} Processors", DateTime.Now - startTime);
                }
                else if (!useThreading)
                {
                    result = FactorialCalculator.SingleProcessor(number);
                    FactorialCalculator.PrintResults(result, "Single Processor", DateTime.Now - startTime);

                }
                else
                {
                    result = FactorialCalculator.Threading(number, Environment.ProcessorCount);
                    FactorialCalculator.PrintResults(result, $"{Environment.ProcessorCount} Processors", DateTime.Now - startTime);
                }

            }
        }
    }
}
