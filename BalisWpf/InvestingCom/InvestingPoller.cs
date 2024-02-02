using System;
using System.Threading.Tasks;
using BalisStandard;

namespace BalisWpf
{
    public class InvestingResults
    {
        public DateTime LastCheck { get; set; }
        public double UsdRub { get; set; }
        public double EurRub { get; set; }
        public double EurUsd { get; set; }
        public double CnyRub { get; set; }
    }

    public class InvestingPoller
    {
        private ShellVm _shellVm;

        public async void Start(ShellVm vm)
        {
            _shellVm = vm;
            var extractor = new InvestingExtractor();
            while (true)
            {
                var rate = await extractor.GetRate("usd-rub");
                _shellVm.InvestingComViewModel.LastCheck = DateTime.Now;
                _shellVm.InvestingComViewModel.UsdRub = rate;

                Console.WriteLine($@"request usd/rub from investing.com at {DateTime.Now} rate {rate}");

                await Task.Delay(1000);

                rate = await extractor.GetRate("eur-rub");
                _shellVm.InvestingComViewModel.LastCheck = DateTime.Now;
                _shellVm.InvestingComViewModel.EurRub = rate;
                Console.WriteLine($@"request euro/rub from investing.com at {DateTime.Now} rate {rate}");

                await Task.Delay(1000);

                rate = await extractor.GetRate("eur-usd");
                _shellVm.InvestingComViewModel.LastCheck = DateTime.Now;
                _shellVm.InvestingComViewModel.EurUsd = rate;
                Console.WriteLine($@"request euro/usd from investing.com at {DateTime.Now} rate {rate}");

                await Task.Delay(1000);

                 rate = await extractor.GetRate("usd-cny");
                _shellVm.InvestingComViewModel.LastCheck = DateTime.Now;
                _shellVm.InvestingComViewModel.UsdCny = rate;
                Console.WriteLine($@"request usd/cny from investing.com at {DateTime.Now} rate {rate}");

                await Task.Delay(1000);

                rate = await extractor.GetRate("cny-rub");
                _shellVm.InvestingComViewModel.LastCheck = DateTime.Now;
                _shellVm.InvestingComViewModel.CnyRub = rate;
                _shellVm.ForecastVm.CalculateNewRates(_shellVm.InvestingComViewModel.Forex);
                Console.WriteLine($@"request cny/rub from investing.com at {DateTime.Now} rate {rate}");

                await Task.Delay(45000);
            }

            // ReSharper disable once FunctionNeverReturns
        }
    }
}
