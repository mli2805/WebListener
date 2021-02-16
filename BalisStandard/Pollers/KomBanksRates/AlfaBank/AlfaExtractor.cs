using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BalisStandard
{
    public class AlfaExtractor : IRatesLineExtractor
    {
        public string BankTitle => KomBankE.Alfa.ToString().ToUpper();
        private const string Url = @"https://www.alfabank.by/exchange/digital/";

        public async Task<KomBankRatesLine> GetRatesLineAsync()
        {
            var mainPage = await ((HttpWebRequest)WebRequest.Create(Url))
                .InitializeForKombanks()
                .GetDataAsync();
            if (string.IsNullOrEmpty(mainPage))
                return null;

            try
            {
                var indexOfStart = mainPage.IndexOf("data-initial='", StringComparison.InvariantCulture) + 14;
                var indexOfEnd = mainPage.IndexOf("'", indexOfStart, StringComparison.InvariantCulture) - 1;
                var length = indexOfEnd - indexOfStart + 1;
                var json = mainPage.Substring(indexOfStart, length);

                return Parse(json);
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} in Prior parser");
                return null;
            }
        }

        private KomBankRatesLine Parse(string json)
        {
            var alfaRoot = JsonConvert.DeserializeObject<AlfaRoot>(json);
            if (alfaRoot == null)
                return null;

            var result = new KomBankRatesLine()
            {
                Bank = BankTitle,
                LastCheck = DateTime.Now,
                StartedFrom = alfaRoot.initialItems[0].currenciesData[0].date,
                UsdA = alfaRoot.initialItems[0].currenciesData[0].value.exchangeRate[0].purchase.value,
                UsdB = alfaRoot.initialItems[0].currenciesData[0].value.exchangeRate[0].sell.value,

                EurA = alfaRoot.initialItems[0].currenciesData[0].value.exchangeRate[1].purchase.value,
                EurB = alfaRoot.initialItems[0].currenciesData[0].value.exchangeRate[1].sell.value,

                RubA = alfaRoot.initialItems[0].currenciesData[0].value.exchangeRate[2].purchase.value,
                RubB = alfaRoot.initialItems[0].currenciesData[0].value.exchangeRate[2].sell.value,

                EurUsdA = alfaRoot.initialItems[0].currenciesData[0].value.conversionRate[0].purchase.value,
                EurUsdB = alfaRoot.initialItems[0].currenciesData[0].value.conversionRate[0].sell.value,

                RubEurA = alfaRoot.initialItems[0].currenciesData[0].value.conversionRate[1].purchase.value,
                RubEurB = alfaRoot.initialItems[0].currenciesData[0].value.conversionRate[1].sell.value,

                RubUsdA = alfaRoot.initialItems[0].currenciesData[0].value.conversionRate[2].purchase.value,
                RubUsdB = alfaRoot.initialItems[0].currenciesData[0].value.conversionRate[2].sell.value,
            };

            return result;
        }
    }
}
