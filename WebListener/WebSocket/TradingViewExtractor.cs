using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebListener
{
    public class TradingViewExtractor
    {
        public ClientWebSocket ClientWebSocket;
        private Uri _uri;
        private CancellationTokenSource _cts;

        private string FrameIt(string request) { return $"~m~{request.Length}~m~{request}"; }
        private const string EurUsdRequest = "{\"p\":[\"my_session\",\"FX_IDC:EURUSD\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string UsdRubRequest = "{\"p\":[\"my_session\",\"FX_IDC:USDRUB\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string EurRubRequest = "{\"p\":[\"my_session\",\"FX_IDC:EURRUB\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string BrentRequest = "{\"p\":[\"my_session\",\"FX:UKOIL\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";

        public TradingViewExtractor()
        {
            ClientWebSocket = new ClientWebSocket();
            ClientWebSocket.Options.UseDefaultCredentials = true;
            //    ClientWebSocket.Options.SetRequestHeader("Host","data.tradingview.com");
            ClientWebSocket.Options.SetRequestHeader("Origin", "https://www.tradingview.com");
            _uri = new Uri("wss://data.tradingview.com/socket.io/websocket");
            _cts = new CancellationTokenSource();
        }

        public async Task RequestRate(TradingViewChart chart)
        {
            switch (chart)
            {
                    case TradingViewChart.EurUsd: await RateRequested(EurUsdRequest); break;
                    case TradingViewChart.UsdRub: await RateRequested(UsdRubRequest); break;
                    case TradingViewChart.EurRub: await RateRequested(EurRubRequest); break;
                    case TradingViewChart.UkOil: await RateRequested(BrentRequest); break;
            }
        }

        private async Task RateRequested(string request)
        {
            try
            {
                var token = _cts.Token;
                var currentRequest = FrameIt(request);
                var bytes = Encoding.UTF8.GetBytes(currentRequest);
                var buffer2 = new ArraySegment<Byte>(bytes, 0, bytes.Length);
                await ClientWebSocket.SendAsync(buffer2, WebSocketMessageType.Text, true, token);
                Console.WriteLine();
                Console.WriteLine("Rate requested.");
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception");
                Console.WriteLine($"{ClientWebSocket.State}");
            }
        }

        public async Task SessionRequested()
        {
            var token = _cts.Token;
            const string sessionRequest = "~m~50~m~{\"p\":[\"my_session\",\"\"],\"m\":\"quote_create_session\"}";
            var encoded = Encoding.UTF8.GetBytes(sessionRequest);
            var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
            await ClientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, token);
            Console.WriteLine();
            Console.WriteLine("Session requested.");
            Console.WriteLine();
        }

        public async Task<bool> ReceiveData()
        {
            try
            {
                var receiveBuffer = new byte[3000];
                var arraySegment = new ArraySegment<byte>(receiveBuffer);
                WebSocketReceiveResult result = await ClientWebSocket.ReceiveAsync(arraySegment, _cts.Token);

                if (result.Count != 0 || result.CloseStatus == WebSocketCloseStatus.Empty)
                {
                    string message = Encoding.ASCII.GetString(arraySegment.Array, arraySegment.Offset, result.Count);
                    Console.WriteLine(message);
                    if (TradingViewParser.TryParse(message, out TradingViewResult tvr))
                        OnResultFetched(tvr); else Console.WriteLine("parsing failed.");
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception");
                Console.WriteLine($"{ClientWebSocket.State}");
                return false;
            }
        }

        public async Task ConnectWebSocket()
        {
            await ClientWebSocket.ConnectAsync(_uri, _cts.Token);
            Console.WriteLine(ClientWebSocket.State);
        }

        public async Task CloseWebSocket()
        {
            await ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, _cts.Token);
            Console.WriteLine(ClientWebSocket.State);
        }

        public event TradingViewResultFetchedEventHandler ResultFetched;
        protected virtual void OnResultFetched(TradingViewResult e)
        {
            ResultFetched?.Invoke(this, e);
        }
    }
}