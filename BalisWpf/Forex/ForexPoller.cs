using System.Threading.Tasks;
using BalisStandard;
using LoadPlaywright;
using UtilsLib;

namespace BalisWpf
{
    public class ForexPoller
    {
        private ShellVm _shellVm;

        public async void Start(ShellVm vm, IMyLog logFile, string playwrightPath)
        {
            _shellVm = vm;
            var extractor = new ProFinanceExtractor(logFile, playwrightPath, false);
            while (true)
            {
                var page = await extractor.Fetch("https://www.profinance.ru/quotes/");
                if (string.IsNullOrEmpty(page))
                    continue;

                var forexRates = ProFinanceParser.Parse(page);
                 _shellVm.ForexViewModel.Update(forexRates);
                _shellVm.ForecastVm.CalculateNewRates(_shellVm.ForexViewModel.RatesForForecast);

                await Task.Delay(45000);
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
