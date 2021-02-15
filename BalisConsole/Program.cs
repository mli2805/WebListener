using System;
using BalisStandard;

namespace BalisConsole
{
    class Program
    {
        static void Main()
        {
            Test();
            Console.ReadKey();
        }

        private static async void Test()
        {
            var extractor = new BnbExtractor();
            var rate = await extractor.GetRatesLineAsync();
            Console.WriteLine(rate);
        }


    }

}
