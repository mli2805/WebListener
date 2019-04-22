using System;
using System.IO;
using System.Threading;
using Extractors;

namespace WebSocketConsole
{
   
    class Program
    {
        static StreamWriter _logFile = File.AppendText("weblistener.log");
        private static TradingViewExtractor _tradingViewExtractor;
        static void Main()
        {
            var ex = new BelgazMobi();
            var res = ex.GetRatesLineAsync().Result;
            Console.ReadKey();
        }

        private static void TradingMain()
        {
            _tradingViewExtractor = new TradingViewExtractor();
            _tradingViewExtractor.CrossRateFetched += TradingViewExtractorCrossRateFetched;

            _tradingViewExtractor.ConnectWebSocket().Wait();
            _tradingViewExtractor.SessionRequested().Wait();
            _tradingViewExtractor.RateRequested().Wait();

            while (true)
            {
                var result = _tradingViewExtractor.ReceiveData().Result;
                if (result)
                {
                    Thread.Sleep(10);
                }
                else
                {
                    _tradingViewExtractor = new TradingViewExtractor();
                    _tradingViewExtractor.CrossRateFetched += TradingViewExtractorCrossRateFetched;
                    _tradingViewExtractor.ConnectWebSocket().Wait();
                    _tradingViewExtractor.SessionRequested().Wait();
                    _tradingViewExtractor.RateRequested().Wait();
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static void TradingViewExtractorCrossRateFetched(object sender, string e)
        {
            Console.WriteLine($"{DateTime.Now}  " + e);
            Console.WriteLine();
            _logFile.WriteLine($"{DateTime.Now}  " + e);
            _logFile.Flush();
        }
    }
}
