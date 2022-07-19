using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BalisStandard
{
    public static class NbRbRatesExtractor
    {
        public static async Task<NbRates> GetNbDayAsync(DateTime date)
        {
            string uri = "https://www.nbrb.by/API/ExRates/Rates?onDate=" + $"{date:yyyy-M-d}" + "&Periodicity=0";
            var json = await ((HttpWebRequest)WebRequest.Create(uri))
                .GetDataAsync();
            if (string.IsNullOrEmpty(json))
                return null;

            var nbList = (List<NbRbSiteRate>)JsonConvert.DeserializeObject(json, typeof(List<NbRbSiteRate>));
            if (nbList == null || nbList.Count == 0) return null;
            var result = new NbRates() { Date = date };
            var usdRate = nbList.First(c => c.Cur_Abbreviation == "USD");
            result.Usd = usdRate.Cur_OfficialRate;
            var euroRate = nbList.First(c => c.Cur_Abbreviation == "EUR");
            result.Eur = euroRate.Cur_OfficialRate;
            var rubRate = nbList.First(c => c.Cur_Abbreviation == "RUB");
            result.Rub = rubRate.Cur_OfficialRate;
            var cnyRate = nbList.First(c => c.Cur_Abbreviation == "CNY");
            result.Cny = cnyRate.Cur_OfficialRate;
            return result;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public class NbRbSiteRate
        {
            public int Cur_ID { get; set; }
            public DateTime Date { get; set; }
            public string Cur_Abbreviation { get; set; }
            public int Cur_Scale { get; set; }
            public string Cur_Name { get; set; }
            public double Cur_OfficialRate { get; set; }
        }
    }

}
