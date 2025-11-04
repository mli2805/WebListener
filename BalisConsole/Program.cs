using System;
using System.IO;
using System.Threading.Tasks;
using BalisStandard;
using LoadPlaywright;
using UtilsLib;

namespace BalisConsole
{
    class Program
    {
        static async Task Main()
        {
            var iniFile = new IniFile();
            iniFile.AssignFile("baliConsole.ini");

            var logFile = new LogFile(iniFile);
            logFile.AssignFile("baliConsole.log");


            //var rate = await NbRbRatesExtractor.GetNbDayAsync(DateTime.Today);

            // var rate = await new BnbExtractor().GetRatesLineAsync();
            // var extractor = new InvestingExtractor();
            // var res = await extractor.GetRatesLineAsync();
            //var rate = await new AlfaExtractor().GetRatesLineAsync();

            //var msPlaywrightPath = iniFile
            //    .Read(IniSection.Extractors, IniKey.MsPlaywrightPath,
            //        @"c:\Users\Professional\AppData\Local\ms-playwright\chromium-1187\chrome-win\chrome.exe");
            //var rate = await new PriorPlaywrightExtractor()
            //    .SetMsPlaywrightPath(msPlaywrightPath)
            //    .SetLogger(logFile)
            //    .GetRatesLineAsync();

            var page = File.ReadAllText("prior.html");
            var rate = PriorFullPageParser.ParseKomBankRatesFromHtml(page);

            //var content = File.ReadAllText("page.html");
            //var rate = AlfaFullPageParser.ParseKomBankRatesFromHtml(content);

            // var rate = await extractor.GetRate( "commodities/brent-oil");
            // var rate = await extractor.GetRate( "currencies/eur-rub");
            Console.WriteLine(rate);
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
