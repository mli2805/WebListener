using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BalisStandard
{
    public class TradingViewExtractor2
    {
        private ClientWebSocket ClientWebSocket;
        private CancellationTokenSource cts;

        private string FrameIt(string request) { return $"~m~{request.Length}~m~{request}"; }
        private const string EurUsdRequest = "{\"p\":[\"my_session\",\"FX_IDC:EURUSD\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string UsdRubRequest = "{\"p\":[\"my_session\",\"FX_IDC:USDRUB\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string EurRubRequest = "{\"p\":[\"my_session\",\"FX_IDC:EURRUB\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string BrentRequest = "{\"p\":[\"my_session\",\"FX:UKOIL\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string VooRequest = "{\"p\":[\"my_session\",\"AMEX:VOO\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";

        public TradingViewExtractor2()
        {
            ClientWebSocket = new ClientWebSocket();
            ClientWebSocket.Options.UseDefaultCredentials = true;
            ClientWebSocket.Options.SetRequestHeader("Origin", "https://www.tradingview.com");
            cts = new CancellationTokenSource();
        }

        public async Task RequestData()
        {
            try
            {
                var token = cts.Token;
                var currentRequest = FrameIt(VooRequest);
                var request = Encoding.UTF8.GetBytes(currentRequest);
                var buffer2 = new ArraySegment<byte>(request, 0, request.Length);
                await ClientWebSocket.SendAsync(buffer2, WebSocketMessageType.Text, true, token);
            }
            catch (Exception e)
            {
                Console.WriteLine($@"Exception {e.Message}");
                Console.WriteLine($"{ClientWebSocket.State}");
            }
        }

        public async Task RequestSession()
        {
            var token = cts.Token;
            const string sessionRequest = "~m~50~m~{\"p\":[\"my_session\",\"\"],\"m\":\"quote_create_session\"}";
            var encoded = Encoding.UTF8.GetBytes(sessionRequest);
            var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
            await ClientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, token);
            Console.WriteLine();
            Console.WriteLine("Session requested.");
            Console.WriteLine();
        }

        private string _remainsOfMessage = "";
        public async Task<bool> ReceiveData()
        {
            try
            {
                var receiveBuffer = new byte[30000];
                var arraySegment = new ArraySegment<byte>(receiveBuffer);
                WebSocketReceiveResult result = await ClientWebSocket.ReceiveAsync(arraySegment, cts.Token);

                if (arraySegment.Array != null && (result.Count != 0 || result.CloseStatus == WebSocketCloseStatus.Empty))
                {
                    string message = Encoding.ASCII.GetString(arraySegment.Array, arraySegment.Offset, result.Count);
                    _remainsOfMessage = ParseSocketData(_remainsOfMessage + message, out List<string> jsonList);
                    OnCrossRateFetched(jsonList);
                }

                return ClientWebSocket.State != WebSocketState.CloseReceived;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception {e.Message}");
                Console.WriteLine($"{ClientWebSocket.State}");
                return false;
            }
        }

        public async Task ConnectWebSocket()
        {
            var uri = new Uri("wss://data.tradingview.com/socket.io/websocket");
            await ClientWebSocket.ConnectAsync(uri, cts.Token);
            Console.WriteLine(ClientWebSocket.State);
        }

        public async Task CloseWebSocket()
        {
            await ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cts.Token);
            Console.WriteLine(ClientWebSocket.State);
        }

        public event DataFetchedEventHandler CrossRateFetched;
        protected virtual void OnCrossRateFetched(List<string> e)
        {
            CrossRateFetched?.Invoke(this, e);
        }

        /// <summary>
        /// splits row data on portions preceded be ~m~ 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="jsonList"></param>
        /// <returns></returns>
        private string ParseSocketData(string message, out List<string> jsonList)
        {
            jsonList = new List<string>();
            while (message.Length > 3)
            {
                var str = message.Substring(3);
                var pos = str.IndexOf("~m~", StringComparison.InvariantCulture);
                var lengthStr = str.Substring(0, pos);
                var length = int.Parse(lengthStr);

                if (message.Length >= length + 3 + 3 + lengthStr.Length)
                {
                    var jsonStr = str.Substring(3 + lengthStr.Length, length);
                    jsonList.Add(jsonStr);
                    message = str.Substring(length + 3 + lengthStr.Length);
                    if (message == "")
                        return "";
                }
                else
                {
                    return message;
                }
            }
            return message;
        }
    }
}