using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Extractors;

namespace WebListener
{
    public class BelgazExtractor
    {
        //  constants in site body { "USD", "EUR", "RUR", "EUR_RUB", "USD_RUB", "USD_EUR" , "EUR_RUB"};
        public async Task<KomBankRates> GetRatesLineAsync()
        {
            var page = await new WebExtractor().GetPageAsync("http://belgazprombank.by/finansovim_institutam/kontaktnaja_informacija/", "utf-8", Encoding.UTF8);
            if (page == "")
                return new KomBankRates { Bank = "БГПБ", StartedFrom = "error" };

            try
            {
                return Parse(page);
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} in Bgpb parser");
                return new KomBankRates { Bank = "БГПБ", StartedFrom = "error" };
            }
        }

        private KomBankRates Parse(string page)
        {
            var table = FetchTable(page);
            if (table == "") return new KomBankRates {Bank = "БГПБ", StartedFrom = "error"};

            var result = new KomBankRates
            {
                Bank = "БГПБ",
                StartedFrom = GetStartedFrom(table),
                LastCheck = DateTime.Now
            };
            ParseRates(table, ref result);
            return result;
        }

        private void ParseRates(string table, ref KomBankRates result)
        {
            double param1, param2;
            GetRateValues(table, "USD", out param1, out param2);
            result.UsdA = param1;
            result.UsdB = param2;
            GetRateValues(table, "EUR", out param1, out param2);
            result.EurA = param1;
            result.EurB = param2;
            GetRateValues(table, "RUR", out param1, out param2);
            result.RubA = param1;
            result.RubB = param2;
            OldGetRateValues(table, "USD_EUR", out param1, out param2);
            result.EurUsdA = param1;
            result.EurUsdB = param2;
            OldGetRateValues(table, "USD_RUB", out param1, out param2);
            result.RubUsdA = param1;
            result.RubUsdB = param2;
            OldGetRateValues(table, "EUR_RUB", out param1, out param2);
            result.RubEurA = param1;
            result.RubEurB = param2;
        }

        private void OldGetRateValues(string table, string key, out double pokupka, out double prodaza)
        {
            var pos = table.IndexOf(key, StringComparison.Ordinal);
            pos = table.IndexOf("<td class=", pos, StringComparison.Ordinal);
            var posFrom = pos + 13;
            var posTo = table.IndexOf("</td>", posFrom, StringComparison.Ordinal);
            var first = table.Substring(posFrom, posTo - posFrom);
            if (!double.TryParse(first, NumberStyles.Any, new CultureInfo("en-US"), out pokupka)) pokupka = -1;

            pos = table.IndexOf("<td class=", posTo, StringComparison.Ordinal);
            posFrom = pos + 13;
            posTo = table.IndexOf("</td>", posFrom, StringComparison.Ordinal);
            var second = table.Substring(posFrom, posTo - posFrom);
            if (!double.TryParse(second, NumberStyles.Any, new CultureInfo("en-US"), out prodaza)) prodaza = -1;

            // с 13/02/2017 дают в прямом формате
//            if (key != "USD_EUR") return;
//            double swap = Math.Round(1 / prodaza, 5);
//            prodaza = Math.Round(1 / pokupka, 5);
//            pokupka = swap;
        }
        private void GetRateValues(string table, string key, out double pokupka, out double prodaza)
        {
            var pos = table.IndexOf(key, StringComparison.Ordinal);
            pos = table.IndexOf("data-byn", pos, StringComparison.Ordinal); // currency name
            pos = table.IndexOf("data-byn", pos+1, StringComparison.Ordinal); // buy
            var posFrom = pos + 10;
            var posTo = table.IndexOf("\"", posFrom, StringComparison.Ordinal);
            var first = table.Substring(posFrom, posTo - posFrom);
            if (!double.TryParse(first, NumberStyles.Any, new CultureInfo("en-US"), out pokupka)) pokupka = -1;

            pos = table.IndexOf("data-byn", posTo, StringComparison.Ordinal); // sell
            posFrom = pos + 10;
            posTo = table.IndexOf("\"", posFrom, StringComparison.Ordinal);
            var second = table.Substring(posFrom, posTo - posFrom);
            if (!double.TryParse(second, NumberStyles.Any, new CultureInfo("en-US"), out prodaza)) prodaza = -1;

            if (key != "USD_EUR") return;
            double swap = Math.Round(1 / prodaza, 5);
            prodaza = Math.Round(1 / pokupka, 5);
            pokupka = swap;
        }
        private string GetStartedFrom(string table)
        {
            var pos = table.IndexOf("Введены в действие", StringComparison.Ordinal);
            pos = table.IndexOf("<span>", pos, StringComparison.Ordinal);
            return table.Substring(pos + 6, 10) + " " + table.Substring(pos + 19, 5);
        }

        private string FetchTable(string webData)
        {
            var pos = webData.IndexOf("Держателям карточек", StringComparison.Ordinal);
            if (pos == -1) return "";
            var endTablePos = webData.IndexOf("/table", pos, StringComparison.Ordinal);
            return webData.Substring(pos, endTablePos - pos);
        }

    }
}
