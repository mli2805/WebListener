using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Extractors
{
    public class BpsOmcExtractor
    {
        private static readonly List<string> Metals = new List<string>() { "ЗОЛОТО", "СЕРЕБРО", "ПЛАТИНА", "ПАЛЛАДИЙ" };

        public async Task<object> GetOmcAsync()
        {
            var page = await new WebExtractorAsync().GetPageAsync("http://www.bps-sberbank.by/43257f17004e948d/dm_rates", "utf-8", Encoding.UTF8);
            if (page == "")
                return new Omc() { FromDate = "error" };

            try
            {
                return Parse(page);
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} in Omc parser");
                return new Omc() { FromDate = "error" };
            }
        }

        private object Parse(string page)
        {
            string table = FetchTable(page);
            if (table == "") return new Omc() {FromDate = "error"};

            var result = new Omc() {FromDate = GetStartedFrom(table), Metals = new Dictionary<string, OmcMetal>()};

            foreach (var metal in Metals)
            {
                var rates = GetMetalRates(table, metal);
                if (rates == null) return null;
                result.Metals.Add(metal,
                    new OmcMetal()
                    {
                        BankBuyUsd = rates[0],
                        BankBuyByn = rates[1],
                        BankSellUsd = rates[2],
                        BankSellByn = rates[3]
                    });
            }

            return result;
        }

        private List<double> GetMetalRates(string table, string key)
        {
            var pos = table.IndexOf(key, StringComparison.Ordinal); // "ЗОЛОТО"
            if (pos == -1) return null;
            var result = new List<double>();

            for (int i = 0; i < 4; i++)
            {
                pos = table.IndexOf("td-text", pos, StringComparison.Ordinal);
                var posFrom = pos + 9;
                var posTo = table.IndexOf("</div>", posFrom, StringComparison.Ordinal);
                var rateString = table.Substring(posFrom, posTo - posFrom);
                var rate = SpecialOperations.ParseDoubleFromWebTrash(rateString);
              //  if (Math.Abs(rate - (-1)) < 0.0001) return null;
                result.Add(rate);
                pos = posTo;
            }
            return result;
        }
        private static string GetStartedFrom(string table)
        {
            var pos = table.IndexOf("<h4>С ", StringComparison.Ordinal);
            if (pos == -1) return "";
            var posTo = table.IndexOf("действуют", pos, StringComparison.Ordinal);
            if (posTo == -1) return "";
            return table.Substring(pos + 4, posTo - pos - 4).Trim();
        }
        private static string FetchTable(string webData)
        {
            var pos = webData.IndexOf("Обезличенные  металлические счета</h1>", StringComparison.Ordinal);
            if (pos == -1) return "";
            var endTablePos = webData.IndexOf("/table", pos, StringComparison.Ordinal);
            //pos = webData.IndexOf("table", endTablePos+5, StringComparison.Ordinal);
            //endTablePos = webData.IndexOf("/table", pos, StringComparison.Ordinal);
            return webData.Substring(pos, endTablePos - pos);
        }
    }
}
