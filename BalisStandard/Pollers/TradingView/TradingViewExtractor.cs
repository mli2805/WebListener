﻿using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BalisStandard
{
    public class TradingViewExtractor
    {
        private readonly TradingViewTiker _tiker;
        private ClientWebSocket _clientWebSocket;
        private CancellationTokenSource _cts;

        private const string TradingViewAddress = "wss://data.tradingview.com/socket.io/websocket";
        private const string SessionRequest = "~m~50~m~{\"p\":[\"my_session\",\"\"],\"m\":\"quote_create_session\"}";

        private readonly Uri _tradingViewUri;
        private readonly ArraySegment<byte> _buffer;

        public TradingViewExtractor(TradingViewTiker tiker)
        {
            _tiker = tiker;

            _tradingViewUri = new Uri(TradingViewAddress);
            var encoded = Encoding.UTF8.GetBytes(SessionRequest);
            _buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);

            _clientWebSocket = new ClientWebSocket();
            _clientWebSocket.Options.UseDefaultCredentials = true;
            _clientWebSocket.Options.SetRequestHeader("Origin", "https://www.tradingview.com");
            _cts = new CancellationTokenSource();
        }

        public async Task ConnectWebSocketAndRequestSession()
        {
            try
            {
                await _clientWebSocket.ConnectAsync(_tradingViewUri, _cts.Token);
                await _clientWebSocket.SendAsync(_buffer, WebSocketMessageType.Text, true, _cts.Token);

            }
            catch (Exception e)
            {
                Console.WriteLine($@"Exception {e.Message}");
            }
        }

      
        public async Task RequestData()
        {
            try
            {
                await _clientWebSocket.SendAsync(_tiker.ToBufferizedRequest(),
                    WebSocketMessageType.Text, true, _cts.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine($@"Exception {e.Message}");
            }
        }


        private string _remainsOfMessage = "";

        public async Task<bool> ReceiveData()
        {
            try
            {
                var receiveBuffer = new byte[30000];
                var arraySegment = new ArraySegment<byte>(receiveBuffer);
                WebSocketReceiveResult result = await _clientWebSocket.ReceiveAsync(arraySegment, _cts.Token);

                if (arraySegment.Array != null && (result.Count != 0 || result.CloseStatus == WebSocketCloseStatus.Empty))
                {
                    string message = Encoding.ASCII.GetString(arraySegment.Array, arraySegment.Offset, result.Count);
                    // Console.WriteLine(message);
                    _remainsOfMessage = ParseSocketData(_remainsOfMessage + message, out List<string> jsonList);
                    OnCrossRateFetched(jsonList);
                }

                return _clientWebSocket.State != WebSocketState.CloseReceived;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception {e.Message}");
                return false;
            }
        }

     
        public event DataFetchedEventHandler CrossRateFetched;
        protected virtual void OnCrossRateFetched(List<string> e)
        {
            CrossRateFetched?.Invoke(this, e);
        }

        /// <summary>
        /// splits row data on portions preceded by ~m~ 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="jsonList"></param>
        /// <returns></returns>
        private string ParseSocketData(string message, out List<string> jsonList)
        {
            jsonList = new List<string>();
            try
            {
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
            catch (Exception)
            {
                return "";
            }
        }
    }
}