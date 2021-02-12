using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace BalisStandard
{
    public class BibExtractor : IRatesLineExtractor
    {
        public string BankTitle => KomBankE.Bib.ToString().ToUpper();
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
            double usdSell;
            if (!GetOneCurrency(table, "USD", out usdBuy, out usdSell))
                return null;
            rates.UsdA = usdBuy;
            rates.UsdB = usdSell;

            double euroBuy;
            double euroSell;
            if (!GetOneCurrency(table, "EUR", out euroBuy, out euroSell))
                return null;
            rates.EurA = euroBuy;
            rates.EurB = euroSell;

            double rubBuy;
            double rubSell;
            if (!GetOneCurrency(table, "RUB", out rubBuy, out rubSell))
                return null;
            rates.RubA = rubBuy;
            rates.RubB = rubSell;

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
            var strSell = table.Substring(pos3 + 62, 10);

            if (!double.TryParse(strBuy, NumberStyles.Any, new CultureInfo("en-US"), out buy)) return false;
            return double.TryParse(strSell, NumberStyles.Any, new CultureInfo("en-US"), out sale);
        }

    }
}
