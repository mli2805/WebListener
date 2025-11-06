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

            var msPlaywrightPath = iniFile
                .Read(IniSection.Extractors, IniKey.MsPlaywrightPath,
                    @"c:\Users\Professional\AppData\Local\ms-playwright\chromium-1187\chrome-win\chrome.exe");
            var savePageToFile = true;

            //var rate = await NbRbRatesExtractor.GetNbDayAsync(DateTime.Today);

            // var rate = await new BnbExtractor().GetRatesLineAsync();
            // var extractor = new InvestingExtractor();
            // var res = await extractor.GetRatesLineAsync();
            //var rate = await new AlfaExtractor().GetRatesLineAsync();


            //var rate = await new PriorPlaywrightExtractor()
            //    .SetMsPlaywrightPath(msPlaywrightPath)
            //    .SetLogger(logFile)
            //    .GetRatesLineAsync();

            //var rate = await new KombankRatesExtractor()
            //    .InitializeExtractor(KomBankE.Bveb, logFile, msPlaywrightPath,
            //        "https://www.belveb.by/rates/upcard/", savePageToFile, new BelVebFullPageParser())
            //    .GetRatesLineAsync();

            //var page = await File.ReadAllTextAsync("prior.html");
            //var rate = new PriorFullPageParser().ParseKomBankRatesFromHtml(page);

            //var content = File.ReadAllText("page.html");
            //var rate = AlfaFullPageParser.ParseKomBankRatesFromHtml(content);

            // var rate = await extractor.GetRate( "commodities/brent-oil");
            // var rate = await extractor.GetRate( "currencies/eur-rub");

            //var xe = new ProFinanceExtractor(logFile, msPlaywrightPath, true);
            //await xe.Fetch("https://www.profinance.ru/quotes/");
            //Console.WriteLine("Done.");

            string html = File.ReadAllText("profinance.html");
            ForexRates rates = ProFinanceParser.Parse(html);

            Console.WriteLine($"EUR/USD: {rates.EurUsd}");
            Console.WriteLine($"USD/CNY: {rates.UsdCny}");
            Console.WriteLine($"Brent Oil: {rates.BrentOil}");
            Console.WriteLine($"Gold: {rates.Gold}");
            Console.WriteLine($"USD/RUB: {rates.UsdRub}");
            Console.WriteLine($"EUR/RUB: {rates.EurRub}");
            Console.WriteLine($"CNY/RUB: {rates.CnyRub}");


            // Console.WriteLine(rate);
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
