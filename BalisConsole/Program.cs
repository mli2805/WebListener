using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using BalisStandard;

namespace BalisConsole
{
    class Program
    {
        static StreamWriter _logFile = File.CreateText("weblistener.log");
        private static TradingViewExtractor _tradingViewExtractor;
        static void Main()
        {
            TradingMain();
            Console.ReadKey();
        }

        // ReSharper disable once UnusedMember.Local
        private static void TradingMain()
        {
            _tradingViewExtractor = new TradingViewExtractor(TradingViewTiker.UkOil);
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
                    _tradingViewExtractor = new TradingViewExtractor(TradingViewTiker.UkOil);
                    _tradingViewExtractor.CrossRateFetched += TradingViewExtractorCrossRateFetched;
                    _tradingViewExtractor.ConnectWebSocketAndRequestSession().Wait();
                    _tradingViewExtractor.RequestData().Wait();
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static void TradingViewExtractorCrossRateFetched(object sender, List<string> e)
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
