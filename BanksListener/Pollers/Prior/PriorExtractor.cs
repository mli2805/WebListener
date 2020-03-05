using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BanksListener
{
    // https://jsoneditoronline.org/
    public class PriorExtractor : IRatesLineExtractor
    {
        private const string RatesRequest =
            "https://www.priorbank.by/main?p_p_id=ExchangeRates_INSTANCE_ExchangeRatesCalculatorView&p_p_lifecycle=2&p_p_state=normal&p_p_mode=view&p_p_resource_id=ajaxSideBarConverterGetRates&p_p_cacheability=cacheLevelPage";


        public async Task<KomBankRatesLine> GetRatesLineAsync()
        {
            var mainPage = await new WebExtractorAsync().GetPageAsync(RatesRequest, "utf-8", Encoding.UTF8);
            if (string.IsNullOrEmpty(mainPage))
                return null;

            try
            {
                var json = mainPage.Replace("\"{", "{").Replace("}\"", "}").Replace(@"\", "");
                return Parse(json);
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} in Prior parser");
                return null;
            }
        }

        private KomBankRatesLine Parse(string page)
        {
            var priorList = (RootObject)JsonConvert.DeserializeObject(page, typeof(RootObject));
            if (priorList.Message != "success")
                return null;

            var result = new KomBankRatesLine { Bank = KomBankE.Prior.ToString().ToUpper() };

            var digitalBank = priorList.CalculatorTable.Data.Where(d => d.Channel == 3).ToList(); // 1 - нал, 2 - карточки

            var onlineRate = digitalBank.First(r => r.BaseCurrency == (int)Currency.Usd && r.RatedCurrency == (int)Currency.Byn);
            result.UsdA = onlineRate.Rate.BuyRate;
            result.UsdB = onlineRate.Rate.SellRate;

            result.StartedFrom = onlineRate.ValidFrom;
            result.LastCheck = DateTime.Now;

            onlineRate = digitalBank.First(r => r.BaseCurrency == (int)Currency.Eur && r.RatedCurrency == (int)Currency.Byn);
            result.EurA = onlineRate.Rate.BuyRate;
            result.EurB = onlineRate.Rate.SellRate;
            onlineRate = digitalBank.First(r => r.BaseCurrency == (int)Currency.Rub && r.RatedCurrency == (int)Currency.Byn);
            result.RubA = onlineRate.Rate.BuyRate;
            result.RubB = onlineRate.Rate.SellRate;

            onlineRate = digitalBank.First(r => r.BaseCurrency == (int)Currency.Usd && r.RatedCurrency == (int)Currency.Rub);
            result.RubUsdA = onlineRate.Rate.BuyRate;
            result.RubUsdB = onlineRate.Rate.SellRate;
            onlineRate = digitalBank.First(r => r.BaseCurrency == (int)Currency.Eur && r.RatedCurrency == (int)Currency.Usd);
            result.EurUsdA = onlineRate.Rate.BuyRate;
            result.EurUsdB = onlineRate.Rate.SellRate;
            onlineRate = digitalBank.First(r => r.BaseCurrency == (int)Currency.Eur && r.RatedCurrency == (int)Currency.Rub);
            result.RubEurA = onlineRate.Rate.BuyRate;
            result.RubEurB = onlineRate.Rate.SellRate;

            return result;
        }


    }
}