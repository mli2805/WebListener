using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BalisStandard;

namespace BalisWpf 
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        public ShellVm Model { get; set; }

        public ShellViewModel()
        {
            Model = new ShellVm(){Test = "in C-tor"};

            Task.Factory.StartNew(() => new TradingViewManager().TradingMain(TradingViewTiker.Voo, Model));

        }

      

    }

    public class TradingViewManager
    {
        private TradingViewExtractor2 _tradingViewExtractor2;
        private ShellVm _vm;
        // ReSharper disable once UnusedMember.Local
        public void TradingMain(TradingViewTiker tiker, ShellVm vm)
        {
            _vm = vm;
            _tradingViewExtractor2 = new TradingViewExtractor2();
            _tradingViewExtractor2.CrossRateFetched += TradingViewExtractor2CrossRateFetched;

            _tradingViewExtractor2.ConnectWebSocket().Wait();
            _tradingViewExtractor2.RequestSession().Wait();
            _tradingViewExtractor2.RequestData().Wait();

            while (true)
            {
                var result = _tradingViewExtractor2.ReceiveData().Result;
                if (result)
                {
                    Thread.Sleep(10);
                }
                else
                {
                    _tradingViewExtractor2 = new TradingViewExtractor2();
                    _tradingViewExtractor2.CrossRateFetched += TradingViewExtractor2CrossRateFetched;
                    _tradingViewExtractor2.ConnectWebSocket().Wait();
                    _tradingViewExtractor2.RequestSession().Wait();
                    _tradingViewExtractor2.RequestData().Wait();
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void TradingViewExtractor2CrossRateFetched(object sender, List<string> e)
        {
            Application.Current.Dispatcher.Invoke(() => ApplyRates(e));
        }

        private void ApplyRates(List<string> e)
        {
            foreach (var json in e)
            {
                var res = TradingViewJsonParser.TryParse(json);
                if (res != null)
                    if (res.ContainsKey("lp"))
                        _vm.Voo = res["lp"].ToString();

                //                foreach (var pair in res)
                //                    {
                //                        Console.WriteLine($"{pair.Key} : {pair.Value.ToString()}");
                //                    }
            }
        }
    }
}