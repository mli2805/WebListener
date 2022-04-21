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
                Console.WriteLine($@"request usd/rub from investing.com at {DateTime.Now}");
                var rate = await extractor.GetRate();
                _vm.TradingViewVm.Rates.UsdRub.Lp = rate;
                _vm.TradingViewVm.LastCheck = DateTime.Now;
                _vm.ForecastVm.CalculateNewRates(_vm.TradingViewVm.Rates);
                Console.WriteLine(rate);

                await Task.Delay(45000);
            }

            // ReSharper disable once FunctionNeverReturns
        }
    }
}
