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
              
                await Task.Delay(7000);

            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
