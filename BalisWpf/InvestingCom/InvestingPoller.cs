using System;
using System.Threading.Tasks;
using BalisStandard;

namespace BalisWpf
{
    public class InvestingPoller
    {
        private ShellVm _shellVm;

        private async Task<double> OneGet(InvestingExtractor extractor, string url)
        {
            var rate = await extractor.GetRate(url);
            _shellVm.InvestingComViewModel.LastCheck = DateTime.Now;
            Console.WriteLine($@"request {url} from investing.com at {DateTime.Now} rate {rate}");
            await Task.Delay(1000);
            return rate;
        }

        public async void Start(ShellVm vm)
        {
            int cycle = 0;
            _shellVm = vm;
            var extractor = new InvestingExtractor();
            while (true)
            {
                cycle++;
                _shellVm.InvestingComViewModel.UsdRub = await OneGet(extractor, "currencies/usd-rub");
                _shellVm.InvestingComViewModel.EurUsd = await OneGet(extractor, "currencies/eur-usd");
                _shellVm.InvestingComViewModel.UsdCny = await OneGet(extractor, "currencies/usd-cny");
                _shellVm.ForecastVm.CalculateNewRates(_shellVm.InvestingComViewModel.Forex);

                if (cycle % 2 == 1)
                {
                    _shellVm.InvestingComViewModel.EurRub = await OneGet(extractor, "currencies/eur-rub");
                    _shellVm.InvestingComViewModel.CnyRub = await OneGet(extractor, "currencies/cny-rub");
                // }
                // else
                // {
                    _shellVm.InvestingComViewModel.Gold = await OneGet(extractor, "commodities/gold");
                    _shellVm.InvestingComViewModel.Brent = await OneGet(extractor, "commodities/brent-oil");
                }

                await Task.Delay(45000);
            }

            // ReSharper disable once FunctionNeverReturns
        }
    }
}
