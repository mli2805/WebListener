using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BalisStandard.AlfaBank;
using Newtonsoft.Json;

namespace BalisStandard
{
    public class AlfaExtractor1 : IRatesLineExtractor
    {
        public string BankTitle => KomBankE.Alfa.ToString().ToUpper();
        private const string Url = @"https://www.alfabank.by/exchange/digital/";

        public async Task<KomBankRatesLine> GetRatesLineAsync()
        {
            var mainPage = await ((HttpWebRequest)WebRequest.Create(Url))
                // .InitializeForKombanks()
                .GetDataAsync();
            if (string.IsNullOrEmpty(mainPage))
                return null;

            try
            {
                var exchangePageStart =  mainPage
                    .IndexOf("\"exchange\":{\"initialItems\":", StringComparison.InvariantCulture) + 12;
               
                var indexOfEnd = mainPage
                    .IndexOf("}'>", exchangePageStart, StringComparison.InvariantCulture) - 1;
                var length = indexOfEnd - exchangePageStart + 1;
                var json = "{" + mainPage.Substring(exchangePageStart, length);

                return Parse(json);
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} in Alfa parser");
                return null;
            }
        }

        private KomBankRatesLine Parse(string json)
        {
            // var alfaRoot = JsonConvert.DeserializeObject<AlfaRoot>(json);
            Root alfaRoot = JsonConvert.DeserializeObject<Root>(json);

            if (alfaRoot == null)
                return null;

            var list = alfaRoot.initialItems[0].currenciesData[0].value.cash;

            var result = new KomBankRatesLine()
            {
                Bank = BankTitle,
                LastCheck = DateTime.Now,
                StartedFrom = DateTime.Parse(alfaRoot.initialItems[0].currenciesData[0].date),


                UsdA = Get(list, "USD", "purchase"),
                UsdB = Get(list, "USD", "sell"),
                
                EurA = Get(list, "EUR", "purchase"),
                EurB = Get(list, "EUR", "sell"),
                
                RubA = Get(list, "RUB", "purchase"),
                RubB = Get(list, "RUB", "sell"),
                
                EurUsdA = Get(list, "EUR/USD", "purchase"),
                EurUsdB = Get(list, "EUR/USD", "sell"),
                
                RubEurA = Get(list, "EUR/RUB", "purchase"),
                RubEurB = Get(list, "EUR/RUB", "sell"),
                
                RubUsdA = Get(list, "USD/RUB", "purchase"),
                RubUsdB = Get(list, "USD/RUB", "sell"),
            };

            return result;
        }

        private double Get(List<Cash> list, string currency, string dest)
        {
            var cur = list.FirstOrDefault(c => c.icon == currency);
            if (cur == null)
            {
                cur= list.First(c => c.title == currency);
            }

            var valueStr = dest == "purchase" ? cur.purchase.value : cur.sell.value;
            return double.Parse(valueStr);
        }
    }

  
}
