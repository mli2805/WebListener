using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BalisStandard
{
    public class BelvebExtractorOld : IRatesLineExtractor
    {
        public string BankTitle => KomBankE.Bveb.ToString().ToUpper();
        private const string MainPage = "https://belveb.by";

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

            var pos = jsonStr.IndexOf("<v-conversion :data-source=\"", StringComparison.Ordinal);
            pos = pos + 28;
            var posEnd = jsonStr.IndexOf("</v-conversion>", StringComparison.Ordinal) - 1;

            var section = jsonStr.Substring(pos, posEnd - pos - 1);

            var ratesPos = section.IndexOf("\"ibank\":{", StringComparison.Ordinal);
            ratesPos += 8;
            var ratesPosEnd = section.IndexOf(",\"offices\":{", StringComparison.Ordinal);
            var jsonRates = section.Substring(ratesPos, ratesPosEnd - ratesPos);
            Root1 exchangeRates = JsonConvert.DeserializeObject<Root1>(jsonRates);

            var convertionPos = section.IndexOf("\"rates\":{", StringComparison.Ordinal);
            var jsonConvertions = "{" + section.Substring(convertionPos, section.Length - convertionPos - 1);
            Root2 convertionRates = JsonConvert.DeserializeObject<Root2>(jsonConvertions);

            var rates = new KomBankRatesLine()
            {
                Bank = KomBankE.Bveb.ToString().ToUpper(),
                StartedFrom = GetStartedFrom(exchangeRates.timetext),
                LastCheck = DateTime.Now
            };

            rates.UsdA = exchangeRates.rates[0].buy_rate.value;
            rates.UsdB = exchangeRates.rates[0].sale_rate.value;

            rates.EurA = exchangeRates.rates[1].buy_rate.value;
            rates.EurB = exchangeRates.rates[1].sale_rate.value;

            rates.RubA = convertionRates.rates.RUB.purchase.BYN * 100;
            rates.RubB = convertionRates.rates.RUB.sell.BYN * 100;
            rates.EurUsdA = convertionRates.rates.EUR.purchase.USD;
            rates.EurUsdB = convertionRates.rates.EUR.sell.USD;
            rates.RubUsdA = convertionRates.rates.USD.purchase.RUB;
            rates.RubUsdB = convertionRates.rates.USD.sell.RUB;
            rates.RubEurA = convertionRates.rates.EUR.purchase.RUB;
            rates.RubEurB = convertionRates.rates.EUR.sell.RUB;

            return rates;
        }

        private static DateTime GetStartedFrom(string timestring)
        {
            try
            {
                var timestampStr = timestring.Substring(10);
                const string format = "dd.MM.yyyy в H:mm";
                var result = DateTime.ParseExact(timestampStr, format, CultureInfo.InvariantCulture);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return DateTime.Now;
            }
        }

    }
}