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
                    vm.BelStockVm.BelStock = stock;

                await Task.Delay(7000);

            }
        }
    }
}
