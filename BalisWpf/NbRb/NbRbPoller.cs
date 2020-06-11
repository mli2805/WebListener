using System;
using System.Threading.Tasks;
using System.Windows;
using BalisStandard;

namespace BalisWpf
{
    public class NbRbPoller
    {
        public async void Start(ShellVm vm)
        {
            while (true)
            {

                if (vm.NbRbViewModel.NbRbVm.Today == null || vm.NbRbViewModel.NbRbVm.Today.Date.Date != DateTime.Today)
                {
                    while (true) // can't step farther till Today is not initialized
                    {
                        var today = await NbRbRatesExtractor.GetNbDayAsync(DateTime.Today);
                        if (today != null)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                vm.NbRbViewModel.NbRbVm.PreviousTradeDay =
                                    today; // force to (re)read Previous day if Today changed
                                vm.NbRbViewModel.NbRbVm.Today = today;
                                vm.BelStockViewModel.TodayNbRates = today;
                                vm.ForecastVm.Initialize(today);
                            });
                            break;
                        }

                        await Task.Delay(60000);
                    }
                }

                while (vm.NbRbViewModel.NbRbVm.PreviousTradeDay.Equals(vm.NbRbViewModel.NbRbVm.Today))
                {
                    var previousTradeDay = await NbRbRatesExtractor.GetNbDayAsync(vm.NbRbViewModel.NbRbVm.PreviousTradeDay.Date.AddDays(-1));
                    if (previousTradeDay != null)
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            vm.NbRbViewModel.NbRbVm.PreviousTradeDay = previousTradeDay;
                            vm.BelStockViewModel.PreviousTradeDayNbRates = previousTradeDay;
                        });
                    else
                        await Task.Delay(60000);
                }

                // ReSharper disable once PossibleNullReferenceException
                if ((vm.NbRbViewModel.NbRbVm.Tomorrow == null || vm.NbRbViewModel.NbRbVm.Tomorrow.Date.Date != DateTime.Today.AddDays(1))
                    && DateTime.Now.Hour >= 13)
                {
                    var tomorrow = await NbRbRatesExtractor.GetNbDayAsync(DateTime.Today.AddDays(1));
                    if (tomorrow != null)
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            vm.NbRbViewModel.NbRbVm.Tomorrow = tomorrow;
                            vm.ForecastVm.Initialize(tomorrow);
                        });
                }

                await Task.Delay(60000);

            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}