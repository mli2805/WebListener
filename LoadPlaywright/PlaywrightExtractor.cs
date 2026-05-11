using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using UtilsLib;

namespace LoadPlaywright
{
    public class PlaywrightExtractor
    {
        private readonly IMyLog _logFile;
        private readonly string _playwrightPath;
        private readonly bool _savePageToFile;

        public PlaywrightExtractor(IMyLog logFile, string playwrightPath, bool savePageToFile)
        {
            _logFile = logFile;
            _playwrightPath = playwrightPath;
            _savePageToFile = savePageToFile;
        }

        public async Task<string?> Fetch(string url)
        {
            _logFile.AppendLine($"Fetch {url}");
            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                ExecutablePath = _playwrightPath,
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

                // не ждем пока WaitUntil = WaitUntilState.NetworkIdle т.к. постоянно обновляет курсы в режиме реального времени
                await page.GotoAsync(url, new PageGotoOptions() );

                // на случай если после выкачки идет долгий рендеринг
                await Task.Delay(1000);

                var content = await page.ContentAsync();
                _logFile.AppendLine($"{url}:: page received successfully");

                if (_savePageToFile)
                {
                    File.WriteAllText($"page.html", content);
                    Console.WriteLine($"HTML сохранён в page.html");
                }

                return content;
            }
            catch (Exception e)
            {
                _logFile.AppendLine($"PlaywrightExtractor: " + e.Message);
            }

            return null;
        }
    }
}