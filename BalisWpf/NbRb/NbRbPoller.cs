using System;
using System.Threading.Tasks;
using BalisStandard;

namespace BalisWpf
{
    public class NbRbPoller
    {
        public async void Start(ShellVm vm)
        {
            while (true)
            {
                if (vm.NbRbVm.Yesterday == null || vm.NbRbVm.Yesterday.Date.Date != DateTime.Today.AddDays(-1))
                {
                    var yesterday = await NbRbRatesExtractor.GetNbDayAsync(DateTime.Today.AddDays(-1));
                    if (yesterday != null)
                        vm.NbRbVm.Yesterday = yesterday;
                    else
                        await Task.Delay(60000);
                }

                if (vm.NbRbVm.Today == null || vm.NbRbVm.Today.Date.Date != DateTime.Today)
                {
                    var today = await NbRbRatesExtractor.GetNbDayAsync(DateTime.Today);
                    if (today != null)
                    {
                        vm.NbRbVm.Today = today;
                        vm.BelStockVm.NbRates = today;
                    }
                }

                if ((vm.NbRbVm.Tomorrow == null || vm.NbRbVm.Tomorrow.Date.Date != DateTime.Today.AddDays(1)) 
                    && DateTime.Now.Hour >= 13)
                {
                    var tomorrow = await NbRbRatesExtractor.GetNbDayAsync(DateTime.Today.AddDays(1));
                    if (tomorrow != null)
                        vm.NbRbVm.Tomorrow = tomorrow;
                }

                await Task.Delay(60000);

            }
        }
    }
}