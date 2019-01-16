using System;
using System.Collections.Generic;
using System.Globalization;
using WebSocket4Net;

namespace WebListener.WebExtractors.Tradingview
{
    class TradingviewExtractor
    {
        private readonly WebSocket _webSocket;
        private readonly CrossRateFetchedEventsArgs _args;

        private string FrameIt(string request) { return $"~m~{request.Length}~m~{request}"; }
        private const string EurUsdRequest = "{\"p\":[\"my_session\",\"FX_IDC:EURUSD\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string UsdRubRequest = "{\"p\":[\"my_session\",\"FX_IDC:USDRUB\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string EurRubRequest = "{\"p\":[\"my_session\",\"FX_IDC:EURRUB\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string BrentRequest = "{\"p\":[\"my_session\",\"FX:UKOIL\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";

        private readonly string _currentRequest;
        public TradingviewExtractor(string key)
        {
            _args = new CrossRateFetchedEventsArgs();

            if (key == "EURUSD") _currentRequest = FrameIt(EurUsdRequest);
            if (key == "USDRUB") _currentRequest = FrameIt(UsdRubRequest);
            if (key == "EURRUB") _currentRequest = FrameIt(EurRubRequest);
            if (key == "UKOIL") _currentRequest = FrameIt(BrentRequest);

            var customHeaderItems = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Host","data.tradingview.com"),
                new KeyValuePair<string, string>("Origin","https://www.tradingview.com"),
            };
            _webSocket = new WebSocket("wss://data.tradingview.com/socket.io/websocket", "", null, customHeaderItems, "", "https://www.tradingview.com");
            _webSocket.Opened += Client_Opened;
            _webSocket.Closed += Client_Closed;
            _webSocket.MessageReceived += Client_MessageReceived;
        }
        public void GetRate()
        {
            try
            {
                    _webSocket.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(@"TradingviewExtractor.GetRate" + e.Message);
            }
        }
        private void Client_Opened(object sender, EventArgs e)
        {
            Console.WriteLine(@"Client_Opened: state is " + _webSocket.State);
            const string sessionRequest = "~m~50~m~{\"p\":[\"my_session\",\"\"],\"m\":\"quote_create_session\"}";
            _webSocket.Send(sessionRequest);
            _webSocket.Send(_currentRequest);
            Console.WriteLine(@">>" + _currentRequest);
        }

        private void Client_Closed(object sender, EventArgs e)
        {
            Console.WriteLine(@"Client_Closed: state is " + _webSocket.State);
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Message);
            var stringRate = ParseMessage(e.Message);
            double rate;
            if (stringRate != "" && double.TryParse(stringRate, NumberStyles.Any, new CultureInfo("en-US"), out rate))
            {
                _args.Rate = rate;
                OnCrossRateFetched(_args);
                _webSocket.Close();
            }
        }

        private string ParseMessage(string message)
        {
            var pos = message.IndexOf("\"lp\":", StringComparison.Ordinal);
            if (pos == -1) return "";
            var posTo = message.IndexOf(",", pos, StringComparison.Ordinal);
            if (posTo == -1) return "";
            var stringRate = message.Substring(pos + 5, posTo - pos - 5);
            return stringRate;
        }

        public event CrossRateFetchedEventHandler CrossRateFetched;
        protected virtual void OnCrossRateFetched(CrossRateFetchedEventsArgs e)
        {
            CrossRateFetched?.Invoke(this, e);
        }
    }
}
