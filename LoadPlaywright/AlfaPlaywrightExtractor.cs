using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;
using BalisStandard;
using UtilsLib;

namespace LoadPlaywright
{
    public class AlfaPlaywrightExtractor : IRatesLineExtractor
    {
        public string BankTitle => KomBankE.Alfa.ToString().ToUpper();

        private string? _msPlaywrightPath;

        private IMyLog? _logFile;

        public AlfaPlaywrightExtractor SetMsPlaywrightPath(string path)
        {
            _msPlaywrightPath = path;
            return this;
        }

        public AlfaPlaywrightExtractor SetLogger(IMyLog logFile)
        {
            _logFile = logFile;
            return this;
        }

        public async Task<KomBankRatesLine?> GetRatesLineAsync()

        {
            _logFile!.AppendLine("ALFA GetRatesLineAsync");
            using var playwright = await Playwright.CreateAsync();
            _logFile.AppendLine("Playwright.CreateAsync");
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                //ExecutablePath = @"c:\Users\builder\AppData\Local\ms-playwright\chromium-1187\chrome-win\chrome.exe",
                ExecutablePath = _msPlaywrightPath,
                Headless = true
            });
            try
            {

                _logFile.AppendLine("browser launched");

                var context = await browser.NewContextAsync(new BrowserNewContextOptions
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
                    ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
                });

                var page = await context.NewPageAsync();
                await page.GotoAsync("https://www.alfabank.by/exchange/digital", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

                // Подождать появления валютной таблицы
                await page.WaitForSelectorAsync("table.table__element");

                _logFile.AppendLine("end of waiting");

                // Получить HTML
                var content = await page.ContentAsync();
                //File.WriteAllText("page.html", content);
                _logFile.AppendLine("content received");

                //Console.WriteLine("HTML сохранён в page.html");

                var rates = AlfaFullPageParser.ParseKomBankRatesFromHtml(content);

                return rates;
            }
            catch (Exception e)
            {
                _logFile.AppendLine("ALFA: " + e.Message);
            }

            return null;
        }
    }
}
