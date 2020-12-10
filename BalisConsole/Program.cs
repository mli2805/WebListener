using System;
using BalisStandard;

namespace BalisConsole
{
    class Program
    {
        static void Main()
        {
            AlfaBank();
            Console.ReadKey();
        }

        private static async void AlfaBank()
        {
            var extractor = new AlfaExtractor();
            var rate = await extractor.GetRatesLineAsync();
            Console.WriteLine(rate);
        }


    }

}
