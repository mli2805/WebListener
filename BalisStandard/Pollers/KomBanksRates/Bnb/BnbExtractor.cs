using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BalisStandard
{
    public class BnbExtractor : IRatesLineExtractor
    {
        public string BankTitle => KomBankE.Bnb.ToString().ToUpper();
        private const string MainPage = "https://bnb.by/kursy-valyut/imbank/";

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
                Console.WriteLine($@"{e.Message} in {BankTitle} parser");
                return null;
            }
        }

        private KomBankRatesLine Parse(string mainPage)
        {
            var indexOfStart = mainPage.IndexOf("currency js-curtable", StringComparison.InvariantCulture) + 11;
            indexOfStart = mainPage.IndexOf("tbody", indexOfStart, StringComparison.InvariantCulture) + 5;
            var indexOfEnd = mainPage.IndexOf("/tbody", indexOfStart, StringComparison.InvariantCulture);
            var length = indexOfEnd - indexOfStart + 1;
            var table = mainPage.Substring(indexOfStart, length);

            var lines = GetTableLines(table);
            var rates = lines.Select(ParseLine).ToList();

            var result = new KomBankRatesLine
            {
                Bank = KomBankE.Bnb.ToString().ToUpper(),
                LastCheck = DateTime.Now
            };

            var usd = rates.FirstOrDefault(r => r.Currency == "USD");
            if (usd != null)
            {
                result.UsdA = usd.Buy;
                result.UsdB = usd.Sell;
            }

            var euro = rates.FirstOrDefault(r => r.Currency == "EUR");
            if (euro != null)
            {
                result.EurA = euro.Buy;
                result.EurB = euro.Sell;
            }

            var rub = rates.FirstOrDefault(r => r.Currency == "100 RUB");
            if (rub != null)
            {
                result.RubA = rub.Buy;
                result.RubB = rub.Sell;
            }

            var euroUsd = rates.FirstOrDefault(r => r.Currency == "EUR/USD");
            if (euroUsd != null)
            {
                result.EurUsdA = euroUsd.Buy;
                result.EurUsdB = euroUsd.Sell;
            }

            var usdRub = rates.FirstOrDefault(r => r.Currency == "USD/RUB");
            if (usdRub != null)
            {
                result.RubUsdA = usdRub.Buy;
                result.RubUsdB = usdRub.Sell;
            }

            var euroRub = rates.FirstOrDefault(r => r.Currency == "EUR/RUB");
            if (euroRub != null)
            {
                result.RubEurA = euroRub.Buy;
                result.RubEurB = euroRub.Sell;
            }


            return result;
        }

        private BnbCurrencyRate ParseLine(string line)
        {
            var element = "<td>";
            var endElement = GetEndElement(element);
            var indexOfStart = line.IndexOf(element, StringComparison.InvariantCulture) + element.Length;
            var indexOfEnd = line.IndexOf(endElement, indexOfStart, StringComparison.InvariantCulture);
            var currency = line.Substring(indexOfStart, indexOfEnd - indexOfStart).Trim();

            indexOfStart = line.IndexOf(element, indexOfEnd, StringComparison.InvariantCulture) + element.Length;
            indexOfEnd = line.IndexOf("<", indexOfStart + 1, StringComparison.InvariantCulture) - 1;
            var buy = line.Substring(indexOfStart, indexOfEnd - indexOfStart + 1).Trim();

            indexOfStart = line.IndexOf(element, indexOfEnd, StringComparison.InvariantCulture) + element.Length;
            indexOfEnd = line.IndexOf("<", indexOfStart + 1, StringComparison.InvariantCulture) - 1;
            var sell = line.Substring(indexOfStart, indexOfEnd - indexOfStart + 1).Trim();

            var result = new BnbCurrencyRate { Currency = currency };
            if (!double.TryParse(buy, out result.Buy))
                result.Buy = 0;
            if (!double.TryParse(sell, out result.Sell))
                result.Sell = 0;

            return result;
        }

        private List<string> GetTableLines(string table)
        {
            var result = new List<string>();

            var element = "<tr>";
            var endElement = GetEndElement(element);

            var indexOfStart = table.IndexOf(element, StringComparison.InvariantCulture) + element.Length;
            var remains = table;
            while (indexOfStart > 0)
            {
                var indexOfEnd = remains.IndexOf(endElement, indexOfStart, StringComparison.InvariantCulture);
                if (indexOfEnd < indexOfStart)
                    Console.WriteLine("");
                var row = remains.Substring(indexOfStart, indexOfEnd - indexOfStart);
                result.Add(row);
                remains = remains.Substring(indexOfEnd + endElement.Length);
                indexOfStart = remains.IndexOf(element, StringComparison.InvariantCulture);
            }

            return result;
        }

        private string GetEndElement(string element)
        {
            return element[0] + "/" + element.Substring(1);
        }
    }
}
