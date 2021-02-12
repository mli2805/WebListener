using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace BalisStandard
{
    public class MtbExtractor : IRatesLineExtractor
    {
        public string BankTitle => KomBankE.Mtb.ToString().ToUpper();
        private const string MainPage = "https://www.mtbank.by/courses/cards/";

        public async Task<KomBankRatesLine> GetRatesLineAsync()
        {
            var mainPage = await ((HttpWebRequest)WebRequest.Create(MainPage))
                .InitializeForKombanks()
                .GetDataAsync();
            if (string.IsNullOrEmpty(mainPage))
                return null;

            try
            {
                return Parse(mainPage);
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} in MTB parser");
                return null;
            }
        }

        private KomBankRatesLine Parse(string table)
        {
            var rates = new KomBankRatesLine()
            {
                Bank = BankTitle,
                LastCheck = DateTime.Now,
            };

            var index = table.IndexOf("rates-table__tbody", StringComparison.Ordinal);

            if (GetOneCurrency(table, index, "usd", out var usdBuy, out var usdSell))
            {
                rates.UsdA = usdBuy;
                rates.UsdB = usdSell;
            }

            if (GetOneCurrency(table, index, "USD/RUB", out var usdRubBuy, out var usdRubSell))
            {
                rates.RubUsdA = usdRubBuy;
                rates.RubUsdB = usdRubSell;
            }

            if (GetOneCurrency(table, index, "eur", out var eurBuy, out var eurSell))
            {
                rates.EurA = eurBuy;
                rates.EurB = eurSell;
            }


            if (GetOneCurrency(table, index, "EUR/USD", out var eurUsdBuy, out var eurUsdSell))
            {
                rates.EurUsdA = eurUsdBuy;
                rates.EurUsdB = eurUsdSell;
            }

            if (GetOneCurrency(table, index, "rub", out var rubBuy, out var rubSell))
            {
                rates.RubA = rubBuy;
                rates.RubB = rubSell;
            }

            if (GetOneCurrency(table, index, "EUR/RUB", out var rubEurBuy, out var rubEurSell))
            {
                rates.RubEurA = rubEurBuy;
                rates.RubEurB = rubEurSell;
            }

            if (GetTimeFrom(table, index, out var timeFrom))
                rates.StartedFrom = timeFrom;

            return rates;
        }

        private static bool GetTimeFrom(string table, int fromIndex, out DateTime startFrom)
        {
            startFrom = DateTime.Today;
            try
            {
                var timeEnd = table.IndexOf("</option>", fromIndex, StringComparison.Ordinal);
                var timeStart = table.LastIndexOf('>', timeEnd, 10);
                var timeStr = table.Substring(timeStart + 1, timeEnd - timeStart - 1);
                var time = TimeSpan.ParseExact(timeStr, "h\\:mm", CultureInfo.InvariantCulture);
                startFrom = DateTime.Today.Add(time);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private static bool GetOneCurrency(string table, int fromIndex, string currency, out double buy, out double sale)
        {
            buy = -1;
            sale = -1;

            try
            {
                var currencyIndex = table.IndexOf(currency, fromIndex, StringComparison.Ordinal);
                var aValue = table.IndexOf("rates-card__double-value", currencyIndex, StringComparison.Ordinal);
                var aEnd = table.IndexOf("</span>", aValue, StringComparison.Ordinal);
                var aStart = table.LastIndexOf('>', aEnd, 10);
                var aStr = table.Substring(aStart + 1, aEnd - aStart - 1);
                if (!double.TryParse(aStr, NumberStyles.Any, new CultureInfo("en-US"), out buy)) return false;


                var bValue = table.IndexOf("rates-card__double-value", aEnd, StringComparison.Ordinal);
                var bEnd = table.IndexOf("</span>", bValue, StringComparison.Ordinal);
                var bStart = table.LastIndexOf('>', bEnd, 10);
                var bStr = table.Substring(bStart + 1, bEnd - bStart - 1);
                return double.TryParse(bStr, NumberStyles.Any, new CultureInfo("en-US"), out sale);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
