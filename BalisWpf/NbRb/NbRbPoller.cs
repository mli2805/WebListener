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
                if (vm.NbRbViewModel.NbRbVm.Yesterday == null || vm.NbRbViewModel.NbRbVm.Yesterday.Date.Date != DateTime.Today.AddDays(-1))
                {
                    var yesterday = await NbRbRatesExtractor.GetNbDayAsync(DateTime.Today.AddDays(-1));
                    if (yesterday != null)
                        vm.NbRbViewModel.NbRbVm.Yesterday = yesterday;
                    else
                        await Task.Delay(60000);
                }

                if (vm.NbRbViewModel.NbRbVm.Today == null || vm.NbRbViewModel.NbRbVm.Today.Date.Date != DateTime.Today)
                {
                    var today = await NbRbRatesExtractor.GetNbDayAsync(DateTime.Today);
                    if (today != null)
                    {
                        vm.NbRbViewModel.NbRbVm.Today = today;
                        vm.BelStockViewModel.NbRates = today;
                        vm.ForecastVm.Initialize(today);
                    }
                }

                if ((vm.NbRbViewModel.NbRbVm.Tomorrow == null || vm.NbRbViewModel.NbRbVm.Tomorrow.Date.Date != DateTime.Today.AddDays(1)) 
                    && DateTime.Now.Hour >= 13)
                {
                    var tomorrow = await NbRbRatesExtractor.GetNbDayAsync(DateTime.Today.AddDays(1));
                    if (tomorrow != null)
                    {
                        vm.NbRbViewModel.NbRbVm.Tomorrow = tomorrow;
                        vm.ForecastVm.Initialize(tomorrow);
                    }
                }

                await Task.Delay(60000);

            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}