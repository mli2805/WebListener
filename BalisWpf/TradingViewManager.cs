using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BalisStandard;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BalisWpf
{
    public class TradingViewManager
    {
        private TradingViewExtractor _tradingViewExtractor;
        private ShellVm _vm;
        private TikerValues _tikerValues;

        public async void Start(TradingViewTiker tiker, TikerValues tikerValues, ShellVm vm, int startDelayMs)
        {
            await Task.Delay(startDelayMs);
            _vm = vm;
            _tikerValues = tikerValues;
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
            var flag = 0;
            foreach (var json in e)
            {
                var res = TradingViewJsonParser.TryParse(json);
                if (res == null)
                    continue;
                flag += ApplyTikerCurrentValues(res);
                flag += ApplyMarketStatus(res);
                flag += ApplyCurrentSession(res);
                flag += ApplyPreMarket(res);
            }
            if (flag > 0)
                _vm.LastCheck = DateTime.Now;
        }

        private int ApplyPreMarket(JObject jObject)
        {
            if (jObject.ContainsKey("rtc"))
                _tikerValues.Rtc = (double)jObject["rtc"];
            if (jObject.ContainsKey("rch"))
                _tikerValues.Rch = (double)jObject["rch"];
            if (jObject.ContainsKey("rchp"))
                _tikerValues.Rchp = (double)jObject["rchp"];
            return 1;
        }

        private int ApplyMarketStatus(JObject jObject)
        {
            if (!jObject.ContainsKey("market-status"))
                return 0;
            var ms = jObject["market-status"].ToString();
            var marketStatus = JsonConvert.DeserializeObject<TradingViewMarketStatusObject>(ms);
            _tikerValues.MarketStatus = marketStatus.Phase;
            return 1;
        }

        private int ApplyCurrentSession(JObject jObject)
        {
            if (jObject.ContainsKey("current_session"))
                _tikerValues.CurrentSession = jObject["current_session"].ToString();
            return 1;
        }

        private int ApplyTikerCurrentValues(JObject jObject)
        {
            if (jObject.ContainsKey("lp"))
                _tikerValues.Lp = (double)jObject["lp"];
            if (jObject.ContainsKey("ch"))
                _tikerValues.Ch = (double)jObject["ch"];
            if (jObject.ContainsKey("chp"))
                _tikerValues.Chp = (double)jObject["chp"];
            return 1;
        }
    }
}