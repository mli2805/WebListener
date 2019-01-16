using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketConsole
{
    public class TradingViewExtractor
    {
        public ClientWebSocket ClientWebSocket;
        private Uri uri;
        private CancellationTokenSource cts;

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
            uri = new Uri("wss://data.tradingview.com/socket.io/websocket");
            cts = new CancellationTokenSource();
        }

        public async Task RateRequested()
        {
            try
            {
                var token = cts.Token;
                var currentRequest = FrameIt(BrentRequest);
                var request = Encoding.UTF8.GetBytes(currentRequest);
                var buffer2 = new ArraySegment<Byte>(request, 0, request.Length);
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
            var token = cts.Token;
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
                WebSocketReceiveResult result = await ClientWebSocket.ReceiveAsync(arraySegment, cts.Token);

                if (result.Count != 0 || result.CloseStatus == WebSocketCloseStatus.Empty)
                {
                    string message = Encoding.ASCII.GetString(arraySegment.Array, arraySegment.Offset, result.Count);
                    OnCrossRateFetched(message);
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
            await ClientWebSocket.ConnectAsync(uri, cts.Token);
            Console.WriteLine(ClientWebSocket.State);
        }

        public async Task CloseWebSocket()
        {
            await ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, cts.Token);
            Console.WriteLine(ClientWebSocket.State);
        }

        public event DataFetchedEventHandler CrossRateFetched;
        protected virtual void OnCrossRateFetched(string e)
        {
            CrossRateFetched?.Invoke(this, e);
        }
    }
}