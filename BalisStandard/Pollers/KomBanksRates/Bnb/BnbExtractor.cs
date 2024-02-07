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


        // на странице всех курсов иногда не обновляют инфу, на главной странице быстрее
        // но на главной странице нет кросс-курсов, поэтому читаем обе страницы и слепливаем
        // 
        private const string MainPage = "https://bnb.by";
        private const string CurrencyPage = "https://bnb.by/kursy-valyut/imbank/";

        public async Task<KomBankRatesLine> GetRatesLineAsync()
        {
            try
            {
                var mainPage = await ((HttpWebRequest)WebRequest.Create(MainPage))
                    .InitializeForKombanks()
                    .GetDataAsync();
                if (string.IsNullOrEmpty(mainPage))
                    return null;

                var ratesOnMainPage = ParseMainPage(mainPage);

                var currencyPage = await ((HttpWebRequest)WebRequest.Create(CurrencyPage))
                    .InitializeForKombanks()
                    .GetDataAsync();
                if (!string.IsNullOrEmpty(currencyPage))
                {
                    var ratesOnCurrencyPage = ParseCurrencyPage(currencyPage);
                    ratesOnMainPage.AddRange(ratesOnCurrencyPage);
                }

                var result = FromRatesList(ratesOnMainPage);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} in {BankTitle} parser");
                return null;
            }
        }

        private List<BnbCurrencyRate> ParseMainPage(string mainPage)
        {
            var pos = 0;
            var rates = new List<BnbCurrencyRate>();

            while (true)
            {
                var posTr = mainPage.IndexOf("rates-table__currency", pos, StringComparison.InvariantCulture);
                if (posTr == -1) break;
                var posEndTr = mainPage.IndexOf("</tr>", posTr, StringComparison.InvariantCulture);
                if (posEndTr == -1) break;
                var tr = mainPage.Substring(posTr, posEndTr - posTr);

                rates.Add(ParseTrFromMainPage(tr));

                pos = posEndTr;
            }

            return rates;
        }

        private BnbCurrencyRate ParseTrFromMainPage(string tr)
        {
            var posEndCurrency = tr.IndexOf("</td>", 0, StringComparison.InvariantCulture);
            var currencyPart = tr.Substring(0, posEndCurrency);
            var posSpace = currencyPart.LastIndexOf(" ", StringComparison.InvariantCulture) + 1;
            var currency = currencyPart.Substring(posSpace, posEndCurrency - posSpace);

            var posValue1 = tr.IndexOf("currency_value", StringComparison.InvariantCulture) + 16;
            var posEndSpan = tr.IndexOf("</span>", posValue1, StringComparison.InvariantCulture);
            var buyStr = tr.Substring(posValue1, posEndSpan - posValue1);
            if (!double.TryParse(buyStr, out double buy)) buy = 0;

            var posValue2 = tr.IndexOf("currency_value", posValue1, StringComparison.InvariantCulture) + 16;
            posEndSpan = tr.IndexOf("</span>", posValue2, StringComparison.InvariantCulture);
            var sellStr = tr.Substring(posValue2, posEndSpan - posValue2);
            if (!double.TryParse(sellStr, out double sell)) sell = 0;

            return new BnbCurrencyRate() { Currency = currency, Buy = buy, Sell = sell };
        }

        private List<BnbCurrencyRate> ParseCurrencyPage(string currencyPage)
        {
            var indexOfStart = currencyPage.IndexOf("currency js-curtable", StringComparison.InvariantCulture) + 11;
            indexOfStart = currencyPage.IndexOf("tbody", indexOfStart, StringComparison.InvariantCulture) + 5;
            var indexOfEnd = currencyPage.IndexOf("/tbody", indexOfStart, StringComparison.InvariantCulture);
            var length = indexOfEnd - indexOfStart + 1;
            var table = currencyPage.Substring(indexOfStart, length);

            var lines = GetTableLines(table);
            return lines.Select(ParseLine).ToList();
        }

        private KomBankRatesLine FromRatesList(List<BnbCurrencyRate> rates)
        {
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
