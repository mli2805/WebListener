﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BalisStandard
{
    public class AlfaExtractorApi : IRatesLineExtractor
    {
        public string BankTitle => KomBankE.Alfa.ToString().ToUpper();
        private const string Url = @"https://developerhub.alfabank.by:8273/partner/1.0.0/public/rates";
        public async Task<KomBankRatesLine> GetRatesLineAsync()
        {
            var page = await ((HttpWebRequest)WebRequest.Create(Url))
                .InitializeForKombanks()
                .GetDataAsync();
            if (string.IsNullOrEmpty(page))
                return null;

            return ParseJson(page);
        }

        private KomBankRatesLine ParseJson(string page)
        {
            AlfaApiRoot list = JsonConvert.DeserializeObject<AlfaApiRoot>(page);
            var result = new KomBankRatesLine();
            result.Bank = BankTitle;
//            result.StartedFrom = DateTime.Now; // v 1.0.1
            result.StartedFrom = list.rates.First().date;

            var usdRate = list.rates.First(r => r.buyCode == 933 && r.sellCode == 840);
            result.UsdA = usdRate.sellRate;
            result.UsdB = usdRate.buyRate;

            var eurRate = list.rates.First(r => r.buyCode == 933 && r.sellCode == 978);
            result.EurA = eurRate.sellRate;
            result.EurB = eurRate.buyRate;

            var rubRate = list.rates.First(r => r.buyCode == 933 && r.sellCode == 643);
            result.RubA = rubRate.sellRate;
            result.RubB = rubRate.buyRate;

            var eurUsdRate = list.rates.First(r => r.buyCode == 840 && r.sellCode == 978);
            result.EurUsdA = eurUsdRate.sellRate;
            result.EurUsdB = eurUsdRate.buyRate;

            var rubUsdRate = list.rates.First(r => r.buyCode == 643 && r.sellCode == 840);
            result.RubUsdA = rubUsdRate.sellRate;
            result.RubUsdB = rubUsdRate.buyRate;

            var rubEurRate = list.rates.First(r => r.buyCode == 643 && r.sellCode == 978);
            result.RubEurA = rubEurRate.sellRate;
            result.RubEurB = rubEurRate.buyRate;

            result.LastCheck = DateTime.Now;
            return result;
        }
    }
}