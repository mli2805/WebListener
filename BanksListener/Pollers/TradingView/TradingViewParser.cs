using System;
using System.Globalization;

namespace BanksListener
{
    public static class TradingViewParser
    {
        private static TradingViewTiker GetByString(string name)
        {
            if (name == "FX_IDC:EURUSD") return TradingViewTiker.EurUsd;
            if (name == "FX_IDC:USDRUB") return TradingViewTiker.UsdRub;
            if (name == "FX_IDC:EURRUB") return TradingViewTiker.EurRub;
            return TradingViewTiker.UkOil; // (name == "FX:UKOIL")
        }

        public static bool TryParse(string message, out TradingViewResult result)
        {
            result = new TradingViewResult();
            if (!TryParseName(message, out string name))
                return false;
            result.Tiker = GetByString(name);
            return TryParseValue(message, out result.Value);
        }

        private static bool TryParseName(string message, out string name)
        {
            name = "";
            var pos = message.IndexOf("\"n\":", StringComparison.Ordinal);
            if (pos == -1)
            {
                return false;
            }
            var posTo = message.IndexOf("\"", pos + 5, StringComparison.Ordinal);
            if (posTo == -1)
            {
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
                return false;
            }
            var stringRate = message.Substring(pos + 5, 15);
            return double.TryParse(stringRate, NumberStyles.Any, new CultureInfo("en-US"), out rate);
        }
    }
}