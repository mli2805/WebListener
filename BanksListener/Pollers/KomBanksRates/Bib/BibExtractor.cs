using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace BanksListener
{
    public class BibExtractor : IRatesLineExtractor
    {
        const string NewMainPage = "https://www.belinvestbank.by/exchange-rates";
        const string NewCardRates = "courses-tab-cashless-content";

        public async Task<KomBankRatesLine> GetRatesLineAsync()
        {
            var mainPage = await ((HttpWebRequest) WebRequest.Create(NewMainPage))
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
                Console.WriteLine($@"{e.Message} in Bib parser");
                return null;
            }
        }

        private KomBankRatesLine Parse(string page)
        {
            var pos = page.IndexOf(NewCardRates, StringComparison.Ordinal);
            return pos == -1 ? null : ParseTable(page.Substring(pos));
        }

      
        private static KomBankRatesLine ParseTable(string table)
        {
            var rates = new KomBankRatesLine()
            {
                Bank = KomBankE.Bib.ToString().ToUpper(),
                StartedFrom = new DateTime(2019, 1, 16, 12, 51, 0),
                LastCheck = DateTime.Now
            };

            double usdBuy;
            double usdSale;
            if (!GetOneCurrency(table, "USD", out usdBuy, out usdSale))
                return null;
            rates.UsdA = usdBuy;
            rates.UsdB = usdSale;

            double euroBuy;
            double euroSale;
            if (!GetOneCurrency(table, "EUR", out euroBuy, out euroSale))
                return null;
            rates.EurA = euroBuy;
            rates.EurB = euroSale;

            double rubBuy;
            double rubSale;
            if (!GetOneCurrency(table, "RUB", out rubBuy, out rubSale))
                return null;
            rates.RubA = rubBuy;
            rates.RubB = rubSale;

            return rates;
        }

        private static bool GetOneCurrency(string table, string currency, out double buy, out double sale)
        {
            buy = -1;
            sale = -1;

            var pos = table.IndexOf(currency, StringComparison.Ordinal);
            var pos2 = table.IndexOf("td_buy", pos, StringComparison.Ordinal);
            var strBuy = table.Substring(pos2 + 61, 10);
            var pos3 = table.IndexOf("td_sell", pos2+10, StringComparison.Ordinal);
            var strSale = table.Substring(pos3 + 62, 10);

            if (!double.TryParse(strBuy, NumberStyles.Any, new CultureInfo("en-US"), out buy)) return false;
            return double.TryParse(strSale, NumberStyles.Any, new CultureInfo("en-US"), out sale);
        }

    }
}
