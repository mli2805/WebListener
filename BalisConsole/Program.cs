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

            // KomBanksPoller.cs
            //var rate = await new KombankRatesExtractor()
            //        .InitializeExtractor(KomBankE.Alfa, logFile, msPlaywrightPath,
            //            "https://www.alfabank.by/exchange/digital", savePageToFile, new AlfaFullPageParser())
            //    .GetRatesLineAsync();

            var parser = new AlfaFullPageParser();
            var content = File.ReadAllText("alfa.html");
            var rate = parser.ParseKomBankRatesFromHtml(content);
            Console.WriteLine(rate);

            //var content = File.ReadAllText("page.html");
            //var rate = AlfaFullPageParser.ParseKomBankRatesFromHtml(content);

            //var parser = new BelStockFullPageParser();
            //// var content = File.ReadAllText("Результаты_торгов_за_сегодня.html");
            //var content = File.ReadAllText("До_торгов_вчерашние_результаты.html");
            //var r = parser.Parse(content);
            //Console.WriteLine(r);

            //var xe = new PlaywrightExtractor(logFile, msPlaywrightPath, true);
            //await xe.Fetch("https://www.profinance.ru/quotes/");
            //Console.WriteLine("Done.");

            // string html = File.ReadAllText("profinance.html");
            // ForexRates rates = ProFinanceParser.Parse(html);
            //
            // Console.WriteLine($"EUR/USD: {rates.EurUsd}");
            // Console.WriteLine($"USD/CNY: {rates.UsdCny}");
            // Console.WriteLine($"Brent Oil: {rates.BrentOil}");
            // Console.WriteLine($"Gold: {rates.Gold}");
            // Console.WriteLine($"USD/RUB: {rates.UsdRub}");
            // Console.WriteLine($"EUR/RUB: {rates.EurRub}");
            // Console.WriteLine($"CNY/RUB: {rates.CnyRub}");


            // Console.WriteLine(rate);
            Console.ReadKey();
        }

    }

}
