using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BalisStandard
{
    public class BelvebExtractor : IRatesLineExtractor
    {
        public string BankTitle => KomBankE.Bveb.ToString().ToUpper();
        private const string MainPage = "https://belveb.by";

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
                Console.WriteLine($@"{e.Message} in BelVEB parser");
                return null;
            }
        }

        private static KomBankRatesLine Parse(string table)
        {
            var rates = new KomBankRatesLine()
            {
                Bank = KomBankE.Bveb.ToString().ToUpper(),
                StartedFrom = GetStartedFrom(table),
                LastCheck = DateTime.Now
            };

            double usdBuy;
            double usdSale;
            if (GetOneCurrency(table, "1 usd", out usdBuy, out usdSale))
            {
                rates.UsdA = usdBuy;
                rates.UsdB = usdSale;
            }

            if (rates.UsdA < 1)
            {
                File.WriteAllText(@"c:/belveb.txt", table);
                return null;
            }

            double euroBuy;
            double euroSale;
            if (GetOneCurrency(table, "1 eur", out euroBuy, out euroSale))
            {
                rates.EurA = euroBuy;
                rates.EurB = euroSale;
            }

            double usdEuro;
            double euroUsd;
            if (GetOneCurrency(table, "EUR/USD", out usdEuro, out euroUsd))
            {
                rates.EurUsdA = usdEuro;
                rates.EurUsdB = euroUsd;
            }

            return rates;
        }

        private static DateTime GetStartedFrom(string mainpage)
        {
            try
            {
                var pos = mainpage.IndexOf("currency-title", StringComparison.Ordinal);
                var timestampStr = mainpage.Substring(pos + 64, 18);
                const string format = "dd.MM.yyyy с h:mm";
                var result = DateTime.ParseExact(timestampStr, format, CultureInfo.InvariantCulture);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return DateTime.Now;
            }
        }

        private static bool GetOneCurrency(string mainpage, string currency, out double buy, out double sale)
        {
            buy = -1;
            sale = -1;

            try
            {
                var pos = mainpage.IndexOf("currency-node\" data-type=\"" + currency, StringComparison.Ordinal);
                var pos1 = mainpage.IndexOf("<span>", pos, StringComparison.Ordinal);
                var pos2 = mainpage.IndexOf("</span>", pos, StringComparison.Ordinal);
                var rateStr = mainpage.Substring(pos1 + 6, pos2 - pos1 - 6);
                var strs = rateStr.Split('/');

                if (!double.TryParse(strs[0], NumberStyles.Any, new CultureInfo("en-US"), out buy)) return false;
                return double.TryParse(strs[1], NumberStyles.Any, new CultureInfo("en-US"), out sale);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}