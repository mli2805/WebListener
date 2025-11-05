using System;
using System.IO;
using System.Threading.Tasks;
using BalisStandard;
using Microsoft.Playwright;
using UtilsLib;

namespace LoadPlaywright
{
    public class PlaywrightExtractor : IRatesLineExtractor
    {
        public string BankTitle => _bank.ToString().ToUpper();

        private KomBankE _bank;
        private IMyLog? _logFile;
        private string? _msPlaywrightPath;
        private string? _url;
        private bool _savePageToFile;
        private IFullPageParser? _fullPageParser;

        public PlaywrightExtractor InitializeExtractor(
            KomBankE bank, IMyLog logFile, string playwrightPath, string url, bool savePageToFile, IFullPageParser fullPageParser)
        {
            _bank = bank;
            _logFile = logFile;
            _msPlaywrightPath = playwrightPath;
            _url = url;
            _savePageToFile = savePageToFile;
            _fullPageParser = fullPageParser;
            return this;
        }

        public async Task<KomBankRatesLine?> GetRatesLineAsync()
        {
            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                ExecutablePath = _msPlaywrightPath,
                Headless = true
            });

            try
            {
                var context = await browser.NewContextAsync(new BrowserNewContextOptions
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
                    ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
                });

                var page = await context.NewPageAsync();
                await page.GotoAsync(_url!, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

                // на случай если после выкачки идет долгий рендеринг
                await Task.Delay(3000); 

                // Получить HTML
                var content = await page.ContentAsync();
                _logFile!.AppendLine($"{_bank.ToString()}:: page received successfully");

                if (_savePageToFile)
                {
                    await File.WriteAllTextAsync($"{_bank.ToString()}.html", content);
                    Console.WriteLine($"HTML сохранён в {_bank.ToString()}.html");
                }

                var rates = _fullPageParser!.ParseKomBankRatesFromHtml(content);

                return rates;
            }
            catch (Exception e)
            {
                _logFile.AppendLine($"{BankTitle}: " + e.Message);
            }

            return null;
        }
    }
}