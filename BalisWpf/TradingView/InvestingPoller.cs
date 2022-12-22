using System;
using System.Threading.Tasks;
using BalisStandard;

namespace BalisWpf
{
    public class InvestingPoller
    {
        private ShellVm _vm;

        public async void Start(ShellVm vm)
        {
            _vm = vm;
            var extractor = new InvestingExtractor();
            while (true)
            {
                var rate = await extractor.GetRate("usd-rub");
                _vm.TradingViewVm.Rates.InvUsdRub.Lp = rate;
                _vm.ForecastVm.CalculateNewRates(_vm.TradingViewVm.Rates);
                Console.WriteLine($@"request usd/rub from investing.com at {DateTime.Now} rate {rate}");

                await Task.Delay(1000);

                rate = await extractor.GetRate("eur-rub");
                _vm.TradingViewVm.Rates.InvEurRub.Lp = rate;
                Console.WriteLine($@"request euro/rub from investing.com at {DateTime.Now} rate {rate}");

                await Task.Delay(1000);

                rate = await extractor.GetRate("cny-rub");
                _vm.TradingViewVm.Rates.InvCnyRub.Lp = rate;
                Console.WriteLine($@"request cny/rub from investing.com at {DateTime.Now} rate {rate}");

                await Task.Delay(45000);
            }

            // ReSharper disable once FunctionNeverReturns
        }
    }
}
