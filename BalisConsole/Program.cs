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
            var extractor = new InvestingExtractor();

            while (true)
            {
                Console.WriteLine($"request at {DateTime.Now}");
                var rate = await extractor.GetRate();
                Console.WriteLine(rate);

                await Task.Delay(15000);
            }

            // ReSharper disable once FunctionNeverReturns
        }

    }

}
