using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Extractors
{
    public class BibExtractor
    {
        const string NewMainPage = "https://www.belinvestbank.by/exchange-rates";
        const string NewCardRates = "courses-tab-cashless-content";

        public async Task<KomBankRates> GetRatesLineAsync()
        {
            var mainPage = await new WebExtractorAsync().GetPageAsync(NewMainPage, "utf-8", Encoding.UTF8);
            if (mainPage == "")
                return new KomBankRates { Bank = "БИБ", StartedFrom = "error" };

            try
            {
                return Parse(mainPage);
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} in Bib parser");
                return new KomBankRates { Bank = "БИБ", StartedFrom = "error" };
            }
        }

        private string GetCardsTable(string page)
        {
            var pos = page.IndexOf(NewCardRates, StringComparison.Ordinal);
       //     var pos2 = page.IndexOf("cards", pos, StringComparison.Ordinal);
            return page.Substring(pos);
        }

        private KomBankRates Parse(string page)
        {
            var table = GetCardsTable(page);

            var result = new KomBankRates()
            {
                Bank = "БИБ",
                StartedFrom = GetStartedFrom(),
                LastCheck = DateTime.Now
            };

            if (ParseTable(table, result))
                return result;

            return new KomBankRates { Bank = "БИБ", StartedFrom = "error" };
        }

        private static bool ParseTable(string table, KomBankRates rates)
        {
            double usdBuy;
            double usdSale;
            if (!GetOneCurrency(table, "USD", out usdBuy, out usdSale))
                return false;
            rates.UsdA = usdBuy;
            rates.UsdB = usdSale;

            double euroBuy;
            double euroSale;
            if (!GetOneCurrency(table, "EUR", out euroBuy, out euroSale))
                return false;
            rates.EurA = euroBuy;
            rates.EurB = euroSale;

            double rubBuy;
            double rubSale;
            if (!GetOneCurrency(table, "RUB", out rubBuy, out rubSale))
                return false;
            rates.RubA = rubBuy;
            rates.RubB = rubSale;

            return true;
        }

        private static bool GetOneCurrency(string table, string currency, out double buy, out double sale)
        {
            buy = -1;
            sale = -1;

            var pos = table.IndexOf(currency, StringComparison.Ordinal);
            var pos2 = table.IndexOf("<b>", pos, StringComparison.Ordinal);
            var strBuy = table.Substring(pos2 + 37, 10);
            var pos3 = table.IndexOf("<b>", pos2+10, StringComparison.Ordinal);
            var strSale = table.Substring(pos3 + 37, 10);

            if (!double.TryParse(strBuy, NumberStyles.Any, new CultureInfo("en-US"), out buy)) return false;
            return double.TryParse(strSale, NumberStyles.Any, new CultureInfo("en-US"), out sale);
        }

        private string GetStartedFrom()
        {
            return $"{new DateTime(2019, 1, 16, 12, 51, 0)}";
        }
    }
}
