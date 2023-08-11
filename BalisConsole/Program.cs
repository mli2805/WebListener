using System;
using System.Threading.Tasks;
using BalisStandard;

namespace BalisConsole
{
    class Program
    {
        static async Task Main()
        {
            var extractor = new BnbExtractor();
            var res = await extractor.GetRatesLineAsync();
            Console.WriteLine(res);
            Console.ReadKey();
        }

        // private const string Url = "http://banki24.by/exchange/currencymarket";
        // private static async Task Test()
        // {
        //     var httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
        //     var initializedRequest = httpWebRequest;
        //     var response = await initializedRequest.GetDataAsync();
        //     Console.WriteLine(response);
        //
        // }

        // private static async Task ArchiveTest()
        // {
        //     var extractor = new Banki24ArchiveExtractor();
        //     var cny = await extractor.GetOneCurrencyDayAsync(new DateTime(2022, 7, 25), Currency.Cny);
        //     Console.WriteLine(cny);
        // }

    }

}
