using System;
using System.Diagnostics;
using System.Globalization;

namespace Extractors
{
    public static class TradingViewParser
    {
        private static TradingViewChart GetByString(string name)
        {
            if (name == "FX_IDC:EURUSD") return TradingViewChart.EurUsd;
            if (name == "FX_IDC:USDRUB") return TradingViewChart.UsdRub;
            if (name == "FX_IDC:EURRUB") return TradingViewChart.EurRub;
            if (name == "AMEX:VOO")
                return TradingViewChart.VOO;
            return TradingViewChart.UkOil; // (name == "FX:UKOIL")
        }

        public static bool TryParse(string message, out TradingViewResult result)
        {
            result = new TradingViewResult();
            if (!TryParseName(message, out string name))
                return false;
            result.Chart = GetByString(name);
            if (result.Chart == TradingViewChart.VOO)
                return TryParseVoo(message, out result);
            else
                return TryParseValue(message, out result.Value);
        }

        private static bool TryParseName(string message, out string name)
        {
            name = "";
            var pos = message.IndexOf("\"n\":", StringComparison.Ordinal);
            if (pos == -1)
            {
                Debug.WriteLine($"TryParseName: pos={pos}");
                return false;
            }
            var posTo = message.IndexOf("\"", pos + 5, StringComparison.Ordinal);
            if (posTo == -1)
            {
                Debug.WriteLine($"TryParseName: posTo={posTo}");
                return false;
            }
            name = message.Substring(pos + 5, posTo - pos - 5);
            return true;
        }

        private static bool TryParseValue(string message, out double rate)
        {
            rate = 0;
            var pos = message.IndexOf("\"lp\":", StringComparison.Ordinal);
            if (pos == -1)
            {
                Debug.WriteLine($"TryParseValue: pos={pos}");
                return false;
            }
            var stringRate = message.Substring(pos + 5, 15);
            return double.TryParse(stringRate, NumberStyles.Any, new CultureInfo("en-US"), out rate);
        }

        private static bool TryParseVoo(string message, out TradingViewResult result)
        {
            result = new TradingViewResult(){ Chart = TradingViewChart.VOO };

            var pos = message.IndexOf("\"lp\":", StringComparison.Ordinal);
            if (pos != -1)
            {
                var stringRate = message.Substring(pos + 5, 6);
                var parse = double.TryParse(stringRate, NumberStyles.Any, new CultureInfo("en-US"), out result.Value);
            }

            pos = message.IndexOf("\"ask\":", StringComparison.Ordinal);
            if (pos != -1)
            {
                var stringRate = message.Substring(pos + 6, 6);
                var parse = double.TryParse(stringRate, NumberStyles.Any, new CultureInfo("en-US"), out result.Ask);
            }

            pos = message.IndexOf("\"bid\":", StringComparison.Ordinal);
            if (pos != -1)
            {
                var stringRate = message.Substring(pos + 6, 6);
                var parse = double.TryParse(stringRate, NumberStyles.Any, new CultureInfo("en-US"), out result.Bid);
            }

            return true;
        }
    }
}