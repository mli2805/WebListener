using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using WebListener.DomainModel;
using WebListener.WebExtractorsAsync;

namespace WebListener
{
    class BpsExtractor
    {
        //  constants in site body { "USD", "EUR", "RUB" };
        public async Task<KomBankRates> GetRatesLineAsync()
        {
            var page = await new WebExtractor().GetPageAsync("http://www.bps-sberbank.by/43257f17004e948d/currency_rates?openform&tabnum=2", "utf-8", Encoding.UTF8);
            if (page == "")
                return new KomBankRates { Bank = "БПС", StartedFrom = "error" };

            return Parse(page);
        }

        private KomBankRates Parse(string page)
        {
            var mainTable = FetchTable(page);
            if (mainTable == "") return new KomBankRates {Bank = "БПС", StartedFrom = "error"};

            var result = new KomBankRates
            {
                Bank = "БПС",
                StartedFrom = GetStartedFrom(mainTable),
                LastCheck = DateTime.Now
            };

            try
            {
                ParseTable(mainTable, ref result);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} in Bps parser");
                return new KomBankRates { Bank = "БПС", StartedFrom = "error" }; 
            }
        }

        private void ParseTable(string table, ref KomBankRates result)
        {
            double param1, param2;
            GetRateValues(table, "1 USD", out param1, out param2);
            result.UsdA = param1;
            result.UsdB = param2;
            GetRateValues(table, "1 EUR", out param1, out param2);
            result.EurA = param1;
            result.EurB = param2;
            GetRateValues(table, "100 RUB", out param1, out param2);
            result.RubA = param1;
            result.RubB = param2;
        }
        private void GetRateValues(string table, string key, out double pokupka, out double prodaza)
        {
            var pos = table.IndexOf(key, StringComparison.Ordinal);
            pos = table.IndexOf("</div>", pos+1, StringComparison.Ordinal);
            pos = table.IndexOf("</div>", pos+1, StringComparison.Ordinal);
            var posFrom = pos + 6;
            var posTo = table.IndexOf("</td>", posFrom, StringComparison.Ordinal);
            var first = table.Substring(posFrom, posTo - posFrom);
            var rateClearString = SpecialOperations.ClearString(first);

            if (!double.TryParse(rateClearString.ToString(), NumberStyles.Any, new CultureInfo("en-US"), out pokupka)) pokupka = -1;

            pos = table.IndexOf("</div>", pos+10, StringComparison.Ordinal);
            pos = table.IndexOf("</div>", pos+1, StringComparison.Ordinal);
            posFrom = pos + 6;
            posTo = table.IndexOf("</td>", posFrom, StringComparison.Ordinal);
            var second = table.Substring(posFrom, posTo - posFrom);
            rateClearString = SpecialOperations.ClearString(second);

            if (!double.TryParse(rateClearString.ToString(), NumberStyles.Any, new CultureInfo("en-US"), out prodaza)) prodaza = -1;
        }
        private string GetStartedFrom(string table)
        {
            int posTo = table.IndexOf("</p>", StringComparison.Ordinal);
            if (posTo == -1) return "error";
            return table.Substring(21, posTo - 21);
        }
        private string FetchTable(string page)
        {
            try
            {
                int posFrom = page.IndexOf("<p>Курсы действуют с", StringComparison.Ordinal);
                int posTo = page.IndexOf("</table>", StringComparison.Ordinal);
                if (posFrom == -1 || posTo == -1) return "";
                return page.Substring(posFrom, posTo - posFrom);
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} in Bps parser");
                return "";
            }
        }
    }
}
