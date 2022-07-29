using System;
using System.Threading.Tasks;
using BalisStandard;

namespace BalisConsole
{
    class Program
    {
        static async Task Main()
        {
            await Test();
            Console.ReadKey();
        }

        private static async Task Test()
        {
            var extractor = new BnbExtractor();

            while (true)
            {
                Console.WriteLine($"request at {DateTime.Now}");
                var rate = await extractor.GetRatesLineAsync();
                Console.WriteLine(rate);

                await Task.Delay(15000);
            }

            // ReSharper disable once FunctionNeverReturns
        }

        private static async Task ArchiveTest()
        {
            var extractor = new Banki24ArchiveExtractor();
            var cny = await extractor.GetOneCurrencyDayAsync(new DateTime(2022, 7, 25), Currency.Cny);
            Console.WriteLine(cny);
        }

    }

}
