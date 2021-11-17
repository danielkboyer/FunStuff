using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FunStuff
{
    /// <summary>
    /// Can calculate extremely large numbers easily
    /// </summary>
    class FactorialCalculator
    {
        static void Main(string[] args)
        {
            bool useThreading = false;
            bool compare = false;
            Console.WriteLine("Use all processors (0), Use Single Processor(1), Compare both(2)");
            string answer = Console.ReadLine();
            if(answer == "1")
            {
                Console.WriteLine("Using a single processor (for large numbers will be slower");

            }
            else if(answer == "0")
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
                    result = SingleProcessor(number);
                    PrintResults(result, "Single Processor", DateTime.Now - startTime);
                    startTime = DateTime.Now;
                    result = Threading(number, Environment.ProcessorCount);
                    PrintResults(result, $"{Environment.ProcessorCount} Processors", DateTime.Now - startTime);
                }
                else if (!useThreading)
                {
                    result = SingleProcessor(number);
                    PrintResults(result, "Single Processor", DateTime.Now - startTime);

                }
                else
                {
                    result = Threading(number, Environment.ProcessorCount);
                    PrintResults(result, $"{Environment.ProcessorCount} Processors", DateTime.Now - startTime);
                }

            }
        }

        static void PrintResults(string result,string type, TimeSpan time)
        {
            Console.WriteLine($"{type} took ({time}): {result}");
        }

        static string SingleProcessor(int number)
        {
            

            string result = "1";

            for (int x = 1; x <= number; x++)
            {
                result = StringMath.Multiply(result, x.ToString());
            }

            return result;
        }

        static string Tasks(int number)
        {

        }

        /// <summary>
        /// This method manually creates x threads based on the amount of logical processors
        /// It splits the initial multiplication evenly among all threads by giving each thread a number and its corresponding smaller number
        /// e.g if number is 200, thread 0 receives 200 and 1. thread 2 receives 199 and 2. and so on.
        /// 
        /// After this initial stage I implemented a reduce search tree which passes the result to another thread who eventually passes to another thread
        /// 
        /// There is a huge problem with this method
        /// comparing the initial multiplication and the reduce tree, the reduce tree takes the longest
        /// the reason is because it is one thread multiplying 2 HUGE numbers and the other threads are idle
        /// 
        /// the solution? we need a different way that can parallize a single multiplication between two values
        /// </summary>
        /// <param name="number"></param>
        /// <param name="processors"></param>
        /// <returns></returns>
        static string Threading(int number, int processors)
        {
            
            (string result, bool done)[] results = new (string result, bool done)[processors];
            Thread[] threads = new Thread[processors];
            List<List<int>> input = new List<List<int>>();

            int[] indexer = new int[processors * 2];
            for(int x = 0; x < processors; x++)
            {
                results[x] = ("1", false);
                indexer[x] = x;
                indexer[processors * 2 - 1 - x] = x;

                input.Add(new List<int>());
            }
            for(int x = number; x >= 1; x--)
            {
                int threadToIndex = indexer[((number-1) - (x-1)) % indexer.Length];
                input[threadToIndex].Add(x);
            }
            for(int x = 0; x < processors; x++)
            {
                threads[x] = new Thread(CalculatePartial);
                threads[x].Start((object)(x, input[x],results,processors));
            }

            for(int x = 0; x < processors; x++)
            {
                threads[x].Join();
            }


            return results[0].result;


        }

        static void CalculatePartial(object data)
        {
            int i = 1;
            (int index, List<int> numbers, (string result, bool done)[] results, int processors) parameters = ((int index, List<int> numbers, (string result, bool done)[] results,int processors))data;

            string result = "1";
            foreach(var number in parameters.numbers)
            {
                result = StringMath.Multiply(number.ToString(), result);
            }

            Console.WriteLine($"Thread {parameters.index} finished");
            while (true)
            {
                int factor = (int)Math.Pow(2, i++);
                if(parameters.index % factor == 0 && parameters.index+factor/2 < parameters.processors)
                {
                    while (!parameters.results[parameters.index+factor/2].done)
                    {
                        
                    }
                    string toMultiply = parameters.results[parameters.index + factor / 2].result;

                    Console.WriteLine($"Thead {parameters.index} received from {parameters.index + factor / 2}");
                    result = StringMath.Multiply(toMultiply, result);
                    Console.WriteLine($"Thead {parameters.index} finished {parameters.index + factor / 2}");

                }
                else if(parameters.index%factor == factor / 2)
                {
                    parameters.results[parameters.index].result = result;
                    parameters.results[parameters.index].done = true;
                    break;
                }
                else if(parameters.index == 0)
                {
                    parameters.results[parameters.index].result = result;
                    break;
                }

            }


        }


        


        
    }


    static class StringMath
    {
        public static string Add(string x, string y)
        {
            if (x.Length == 0)
                return y;
            if (y.Length == 0)
                return x;
            int yLength = y.Length;
            int xLength = x.Length;

            string small = y;
            string large = x;
            if (xLength < yLength)
            {
                large = y;
                small = x;

            }
            int carry = 0;
            string toReturn = "";
            for (int i = 0; i < small.Length; i++)
            {
                int smallPos = small.Length - 1 - i;
                int largePos = large.Length - 1 - i;

                int smallV = small[smallPos] - '0';
                int largeV = large[largePos] - '0';

                int result = smallV + largeV + carry;
                carry = result / 10;
                toReturn = result % 10 + toReturn;
            }
            //for the remainder of large
            for (int i = large.Length - small.Length - 1; i >= 0; i--)
            {
                int largeV = large[i] - '0';

                int result = largeV + carry;
                carry = result / 10;
                toReturn = result % 10 + toReturn;
            }
            if (carry > 0)
                toReturn = carry + toReturn;


            return toReturn;
        }


        private static string SingleM(int x, string y, string zeroes)
        {
            string toBuild = zeroes;

            int carry = 0;


            for (int i = y.Length - 1; i >= 0; i--)
            {
                int yM = y[i] - '0';

                int mResult = x * yM + carry;

                carry = mResult / 10;
                toBuild = mResult % 10 + toBuild;

            }
            if (carry > 0)
                toBuild = carry + toBuild;

            return toBuild;
        }

        public static async Task<string> ThreadedMultiply(string x, string y)
        {
            List<Task<string>> tasks = new List<Task<string>>();
            string toReturn = "";
            for (int p = y.Length - 1; p >= 0; p--)
            {
                string toBuild = "";
                //this loop is for adding the leading zeros when multiplying
                for (int addZeros = y.Length - 1 - p; addZeros > 0; addZeros--)
                {
                    toBuild += "0";
                }
           
                int yM = y[p] - '0';

                tasks.Add(Task.Run(()=>SingleM(yM, x, toBuild)));

                toReturn = Add(toReturn, toBuild);
            }

            while (tasks.Count > 0)
            {
                var task = await Task.WhenAny(tasks);
                toReturn = Add(toReturn,(await task));
                tasks.Remove(task);
            }

            return toReturn;
        }
        public static string Multiply(string x, string y)
        {
            string toReturn = "";
            for (int p = y.Length - 1; p >= 0; p--)
            {
                string toBuild = "";
                //this loop is for adding the leading zeros when multiplying
                for (int addZeros = y.Length - 1 - p; addZeros > 0; addZeros--)
                {
                    toBuild += "0";
                }
                int carry = 0;

                int yM = y[p] - '0';

                for (int i = x.Length - 1; i >= 0; i--)
                {
                    int xM = x[i] - '0';

                    int mResult = xM * yM + carry;

                    carry = mResult / 10;
                    toBuild = mResult % 10 + toBuild;

                }
                if (carry > 0)
                    toBuild = carry + toBuild;

                toReturn = Add(toReturn, toBuild);
            }

            return toReturn;
        }
    }
}
