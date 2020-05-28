using System.Threading.Tasks;
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
                {
                    if (stock.TradingState == BelStockState.TerminatedAlready 
                        && vm.BelStockViewModel.BelStock.TradingState != BelStockState.TerminatedAlready)
                    {
                        //TODO: forecast initialize with new basket
                    }
                    vm.BelStockViewModel.BelStock = stock;
                }

                await Task.Delay(7000);

            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
