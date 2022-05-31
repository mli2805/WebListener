using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BalisStandard
{
    public class BelvebExtractor : IRatesLineExtractor
    {
        public string BankTitle => KomBankE.Bveb.ToString().ToUpper();
        private const string MainPage = "https://belveb.by/rates/ibank";
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
                Console.WriteLine($@"{e.Message} in BelVEB parser");
                return null;
            }

        }

        private static KomBankRatesLine Parse(string page)
        {
            var jsonStr = page.Replace("&quot;", "\"");
            var pos = jsonStr.IndexOf("allCurrencyLink", StringComparison.Ordinal) + 20;
            var posEnd = jsonStr.IndexOf("</v-currency>", StringComparison.Ordinal) - 3;
            var section = jsonStr.Substring(pos, posEnd - pos - 1);

            section = '{' + section;
            BelVebRoot3 exchangeRates = JsonConvert.DeserializeObject<BelVebRoot3>(section);

            var rates = new KomBankRatesLine()
            {
                Bank = KomBankE.Bveb.ToString().ToUpper(),
                LastCheck = DateTime.Now
            };

            string firstTime;
            DateTime startedFrom;
            if (exchangeRates.time == null) // new date
            {
                var firstRate = exchangeRates.currency.First();
                firstTime = firstRate.time;
                if (!DateTime.TryParse(firstTime, out startedFrom)) return null;
                startedFrom = startedFrom.AddDays(-1);
            }
            else
            {
                var firstTimeC = exchangeRates.time.FirstOrDefault();
                if (firstTimeC == null) return null;
                firstTime = firstTimeC.value;
                if (!DateTime.TryParse(firstTime, out startedFrom)) return null;

            }


            rates.StartedFrom = startedFrom;

            var thisTimeRates = exchangeRates.currency.Where(c => c.time == firstTime).ToList();
            var usd = thisTimeRates.FirstOrDefault(r => r.currency_cod == "USD");
            if (usd == null) return null;
            var euro = thisTimeRates.FirstOrDefault(r => r.currency_cod == "EUR");
            if (euro == null) return null;
            var euroUsd = thisTimeRates.FirstOrDefault(r => r.currency_cod == "EUR/USD");
            if (euroUsd == null) return null;

            if (!double.TryParse(usd.buy_rate.value, out double a)) return null;
            rates.UsdA = a;
            if (!double.TryParse(usd.sale_rate.value, out a)) return null;
            rates.UsdB = a;
            if (!double.TryParse(euro.buy_rate.value, out a)) return null;
            rates.EurA = a;
            if (!double.TryParse(euro.sale_rate.value, out a)) return null;
            rates.EurB = a; 
            if (!double.TryParse(euroUsd.buy_rate.value, out a)) return null;
            rates.EurUsdA = a;
            if (!double.TryParse(euroUsd.sale_rate.value, out a)) return null;
            rates.EurUsdB = a;

            return rates;
        }
    }
}