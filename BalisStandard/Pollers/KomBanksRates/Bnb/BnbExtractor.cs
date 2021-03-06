﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BalisStandard
{
    public class BnbExtractor : IRatesLineExtractor
    {
        public string BankTitle => KomBankE.Bnb.ToString().ToUpper();
        private const string MainPage = "https://bnb.by/";

        public async Task<KomBankRatesLine> GetRatesLineAsync()
        {
            var mainPage = await ((HttpWebRequest)WebRequest.Create(MainPage))
                .InitializeForKombanks()
                .GetDataAsync();
            if (string.IsNullOrEmpty(mainPage))
                return null;

            try
            {
                var indexOfStart = mainPage.IndexOf("data-date='", StringComparison.InvariantCulture) + 11;
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
            var bnbRoot = JsonConvert.DeserializeObject<Dictionary<int, Object>>(json);
            if (bnbRoot == null)
                return null;
            var imBankingRates = JsonConvert.DeserializeObject<Dictionary<string, ImBankingCurrency>>(bnbRoot[8].ToString());
            var convertionRates = JsonConvert.DeserializeObject<Dictionary<int, BnbCurrency>>(bnbRoot[7].ToString());

            var result = new KomBankRatesLine
            {
                Bank = BankTitle,
                LastCheck = DateTime.Now,
                UsdA = imBankingRates["USD"].UF_BUY,
                UsdB = imBankingRates["USD"].UF_SALE,
                EurA = imBankingRates["EUR"].UF_BUY,
                EurB = imBankingRates["EUR"].UF_SALE,
                RubA = imBankingRates["RUB"].UF_BUY,
                RubB = imBankingRates["RUB"].UF_SALE,

                EurUsdA = convertionRates[310].UF_BUY,
                EurUsdB = convertionRates[310].UF_SALE,
                RubUsdA = convertionRates[308].UF_BUY,
                RubUsdB = convertionRates[308].UF_SALE,
                RubEurA = convertionRates[309].UF_BUY,
                RubEurB = convertionRates[309].UF_SALE,
            };
            return result;
        }
    }
}
