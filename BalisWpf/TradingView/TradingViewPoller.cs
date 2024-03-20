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
    public class TradingViewPoller
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

            _tradingViewExtractor.ConnectWebSocketAndRequestSession().Wait();
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
                    Thread.Sleep(100);
                    _tradingViewExtractor = new TradingViewExtractor(tiker);
                    _tradingViewExtractor.CrossRateFetched += TradingViewExtractorCrossRateFetched;
                    _tradingViewExtractor.ConnectWebSocketAndRequestSession().Wait();
                    _tradingViewExtractor.RequestData().Wait();
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void TradingViewExtractorCrossRateFetched(object sender, List<string> e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => ApplyRates(e));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
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

            _vm.TradingViewVm.LastCheck = DateTime.Now;
        }

        private int ApplyPreMarket(JObject jObject)
        {
            JToken rtcToken = jObject["rtc"];
            if (!rtcToken.IsNullOrEmpty())
                _tikerValues.Rtc = (double)jObject["rtc"];

            JToken rchToken = jObject["rch"];
            if (!rchToken.IsNullOrEmpty())
                _tikerValues.Rch = (double)jObject["rch"];

            JToken rchpToken = jObject["rchp"];
            if (!rchpToken.IsNullOrEmpty())
                _tikerValues.Rchp = (double)jObject["rchp"];

            return 1;
        }

        private int ApplyMarketStatus(JObject jObject)
        {
            if (!jObject.ContainsKey("market-status"))
                return 0;
            var ms = jObject["market-status"].ToString();
            var marketStatus = JsonConvert.DeserializeObject<TradingViewMarketStatusObject>(ms);
            if (marketStatus == null) return 0;
            _tikerValues.MarketStatus = marketStatus.Phase;
            return 1;
        }

        private int ApplyCurrentSession(JObject jObject)
        {
            if (jObject.TryGetValue("current_session", out var value))
                _tikerValues.CurrentSession = value.ToString();
            if (jObject.TryGetValue("prev_close_price", out var value1))
                _tikerValues.PrevClosePrice = (double)value1;
            if (jObject.TryGetValue("open_price", out var value2))
                _tikerValues.OpenPrice = (double)value2;
            if (jObject.TryGetValue("open_time", out var value3))
            {
                var ms = (int)value3;
                TimeSpan time = TimeSpan.FromSeconds(ms);
                DateTime startdate = new DateTime(1970, 1, 1) + time;
                _tikerValues.OpenTime = startdate;
            }
            if (jObject.TryGetValue("timezone", out var value4))
                _tikerValues.TimeZone = value4.ToString();
            return 1;
        }

        private int ApplyTikerCurrentValues(JObject jObject)
        {
            if (jObject.TryGetValue("lp", out var value))
                _tikerValues.Lp = (double)value;
            if (jObject.TryGetValue("ch", out var value1))
                _tikerValues.Ch = (double)value1;
            if (jObject.TryGetValue("chp", out var value2))
                _tikerValues.Chp = (double)value2;
            return 1;
        }
    }

    public static class JsonExtensions
    {
        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }
    }
}