using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BalisLibrary
{
    public class BpsExtractor : IRatesLineExtractor
    {
        public async Task<KomBankRatesLine> GetRatesLineAsync()
        {
            var json = await GetJsonAsync();
            if (string.IsNullOrEmpty(json))
                return null;

            return Parse(json);
        }

        private KomBankRatesLine Parse(string json)
        {
            try
            {
                var result = new KomBankRatesLine() { Bank = KomBankE.Bps.ToString().ToUpper() };
                var bpsRootObject = (BpsRootObject)JsonConvert.DeserializeObject(json, typeof(BpsRootObject));
                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                result.StartedFrom = start.AddMilliseconds(bpsRootObject.date).ToLocalTime();

                var usdRate = bpsRootObject.rates.list.First(r => r.iso == "USD");
                result.UsdA = usdRate.buy;
                result.UsdB = usdRate.sale;
                var eurRate = bpsRootObject.rates.list.First(r => r.iso == "EUR");
                result.EurA = eurRate.buy;
                result.EurB = eurRate.sale; 
                var rubRate = bpsRootObject.rates.list.First(r => r.iso == "RUB");
                result.RubA = rubRate.buy;
                result.RubB = rubRate.sale;

                result.LastCheck = DateTime.Now;
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} in Bps parser");
                return null;
            }
        }

        private async Task<string> GetJsonAsync()
        {
            try
            {
                HttpClient client = new HttpClient();
                var url = "https://www.bps-sberbank.by/SBOLServer/rest/currency/rates?pck=CD";
                var content = new FormUrlEncodedContent(new Dictionary<string, string>());
                var response = await client.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} in Bps extractor");
                return "";
            }
        }
    }
}
