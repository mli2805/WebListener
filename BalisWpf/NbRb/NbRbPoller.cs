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
                if (vm.NbRbData.Yesterday == null || vm.NbRbData.Yesterday.Date.Date != DateTime.Today.AddDays(-1))
                {
                    var yesterday = await NbRbRatesExtractor.GetNbDayAsync(DateTime.Today.AddDays(-1));
                    if (yesterday != null)
                        vm.NbRbData.Yesterday = yesterday;
                }

                if (vm.NbRbData.Today == null || vm.NbRbData.Today.Date.Date != DateTime.Today)
                {
                    var today = await NbRbRatesExtractor.GetNbDayAsync(DateTime.Today);
                    if (today != null)
                        vm.NbRbData.Today = today;
                }

                if ((vm.NbRbData.Tomorrow == null || vm.NbRbData.Tomorrow.Date.Date != DateTime.Today.AddDays(1)) 
                    && DateTime.Now.Hour >= 13)
                {
                    var tomorrow = await NbRbRatesExtractor.GetNbDayAsync(DateTime.Today.AddDays(1));
                    if (tomorrow != null)
                        vm.NbRbData.Tomorrow = tomorrow;
                }

            }
        }
    }
}