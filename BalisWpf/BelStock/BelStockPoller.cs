using System;
using System.Threading.Tasks;
using System.Windows;
using BalisStandard;

namespace BalisWpf
{
    public class BelStockPoller
    {
        public async void Start(ShellVm vm)
        {
            var extractor = new Banki24Extractor();
            var start = new TimeSpan(9, 58, 0);
            var end = new TimeSpan(13, 10, 0);

            while (true)
            {
                var stock = await extractor.GetStockAsync();
                if (stock != null)
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (stock.TradingState == BelStockState.TerminatedAlready 
                            && vm.BelStockViewModel.BelStock.TradingState != BelStockState.TerminatedAlready)
                        {
                       
                            vm.ForecastVm.Initialize(stock.GetTomorrow());
                        }
                        vm.BelStockViewModel.BelStock = stock;
                    });

                
                var now = DateTime.Now.TimeOfDay;
                var gap = ((now > start) && (now < end)) ? 60_000 : 15 * 60_000;
                await Task.Delay(gap);

            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
