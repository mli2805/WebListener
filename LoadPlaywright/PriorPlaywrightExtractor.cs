using System;
using System.IO;
using System.Threading.Tasks;
using BalisStandard;
using Microsoft.Playwright;
using UtilsLib;

namespace LoadPlaywright
{
    public class PriorPlaywrightExtractor : IRatesLineExtractor
    {
        public string BankTitle => KomBankE.Prior.ToString().ToUpper();

        private string? _msPlaywrightPath;

        private IMyLog? _logFile;

        public PriorPlaywrightExtractor SetMsPlaywrightPath(string path)
        {
            _msPlaywrightPath = path;
            return this;
        }

        public PriorPlaywrightExtractor SetLogger(IMyLog logFile)
        {
            _logFile = logFile;
            return this;
        }

        public async Task<KomBankRatesLine?> GetRatesLineAsync()
        {
            _logFile!.AppendLine("Prior GetRatesLineAsync");
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
                await page.GotoAsync("https://www.priorbank.by/offers/services/currency-exchange", 
                    new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

                // на случай если после выкачки идет долгий рендеринг
                await Task.Delay(3000); 

               _logFile.AppendLine("end of waiting");

                // Получить HTML
                var content = await page.ContentAsync();
                File.WriteAllText("prior.html", content);
                _logFile.AppendLine("content received");

                Console.WriteLine("HTML сохранён в prior.html");

                var rates = PriorFullPageParser.ParseKomBankRatesFromHtml(content);

                return rates;
            }
            catch (Exception e)
            {
                _logFile.AppendLine("Prior: " + e.Message);
            }

            return null;
        }
    }
}