using System.Collections.Generic;
using System.Threading;
using System.Windows;
using BalisStandard;

namespace BalisWpf
{
    public class TradingViewManager
    {
        private TradingViewExtractor _tradingViewExtractor;
        private ShellVm _vm;
        private TradingViewTiker _tiker;

        public void TradingMain(TradingViewTiker tiker, ShellVm vm)
        {
            _vm = vm;
            _tiker = tiker;
            _tradingViewExtractor = new TradingViewExtractor(tiker);
            _tradingViewExtractor.CrossRateFetched += TradingViewExtractorCrossRateFetched;

            _tradingViewExtractor.ConnectWebSocket().Wait();
            _tradingViewExtractor.RequestSession().Wait();
            _tradingViewExtractor.RequestData().Wait();

            while (true)
            {
                var result = _tradingViewExtractor.ReceiveData().Result;
                if (result)
                {
                    Thread.Sleep(10);
                }
                else
                {
                    _tradingViewExtractor = new TradingViewExtractor(tiker);
                    _tradingViewExtractor.CrossRateFetched += TradingViewExtractorCrossRateFetched;
                    _tradingViewExtractor.ConnectWebSocket().Wait();
                    _tradingViewExtractor.RequestSession().Wait();
                    _tradingViewExtractor.RequestData().Wait();
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void TradingViewExtractorCrossRateFetched(object sender, List<string> e)
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
                       ApplyLp(res["lp"].ToString());

                //                foreach (var pair in res)
                //                    {
                //                        Console.WriteLine($"{pair.Key} : {pair.Value.ToString()}");
                //                    }
            }
        }

        private void ApplyLp(string lp)
        {
            switch (_tiker)
            {
                case TradingViewTiker.EurUsd:
                    _vm.EurUsd = lp;
                    break;
                case TradingViewTiker.Voo:
                    _vm.Voo = lp;
                    break;
                default: _vm.Test = lp;
                    break;
            }
        }
    }
}