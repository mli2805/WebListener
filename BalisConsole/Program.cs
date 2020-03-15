using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using BalisLibrary;

namespace BalisConsole
{
    class Program
    {
        static StreamWriter _logFile = File.CreateText("weblistener.log");
        private static TradingViewExtractor2 _tradingViewExtractor2;
        static void Main()
        {
            TradingMain();
            Console.ReadKey();
        }

        // ReSharper disable once UnusedMember.Local
        private static void TradingMain()
        {
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

        private static void TradingViewExtractor2CrossRateFetched(object sender, List<string> e)
        {
            Console.WriteLine($"{DateTime.Now}  json strings received: " + e.Count);
            foreach (var json in e)
            {
                _logFile.WriteLine(json);
                _logFile.WriteLine();
                _logFile.Flush();
                var res = TradingViewJsonParser.TryParse(json);
                if (res != null)
                    if (res.ContainsKey("lp"))
                        Console.WriteLine($"lp : {(double)res["lp"]}");

                //                foreach (var pair in res)
                //                    {
                //                        Console.WriteLine($"{pair.Key} : {pair.Value.ToString()}");
                //                    }
            }
        }

    }

}
