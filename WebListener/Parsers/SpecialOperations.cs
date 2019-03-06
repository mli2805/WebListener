using System;
using System.Globalization;
using System.Text;

namespace WebListener
{
    public class SpecialOperations
    {
        public static double ParseDoubleFromWebTrash(string rateString)
        {
            var rateClearString = new StringBuilder();
            foreach (var symbol in rateString)
            {
                if (char.IsNumber(symbol)) rateClearString.Append(symbol);
                else if (symbol == ',' || symbol == '.')
                    rateClearString.Append(CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator);
            }
            double rate;
            return double.TryParse(rateClearString.ToString(), NumberStyles.Any, new CultureInfo("en-US"), out rate) ? rate : -1;
        }

        public static StringBuilder ClearString(string rateString)
        {
            var rateClearString = new StringBuilder();
            foreach (var symbol in rateString)
            {
                if (char.IsNumber(symbol)) rateClearString.Append(symbol);
                else if (symbol == ',' || symbol == '.')
                    rateClearString.Append(CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator);
            }
            return rateClearString;
        }
    }
}