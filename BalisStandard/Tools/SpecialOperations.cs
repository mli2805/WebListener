using System.Globalization;
using System.Text;

namespace BalisStandard
{
    public static class SpecialOperations
    {
        public static double ParseDoubleFromWebTrash(this string rateString)
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

        public static string GetParentFolder(this string path)
        {
            var index = path.LastIndexOf('\\');
            if (index == -1) return null;

            return path.Substring(0, index);
        }
    }
}