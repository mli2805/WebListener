using System;
using System.Threading.Tasks;
using System.Windows;
using BalisStandard;
using UtilsLib;

namespace BalisWpf
{
    public class BelStockPoller
    {
        public async void Start(ShellVm vm, IMyLog logFile, string playwrightPath)
        {
            var extractor = new BelStockExtractorAndParser(logFile, playwrightPath);
            var start = new TimeSpan(9, 58, 0);
            var end = new TimeSpan(13, 25, 0);

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
                var gap = ((now > start) && (now < end)) ? 5 * 60_000 : 35 * 60_000;
                await Task.Delay(gap);

            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
