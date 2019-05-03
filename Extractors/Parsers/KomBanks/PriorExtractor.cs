using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Extractors
{
    // https://jsoneditoronline.org/
    public class PriorExtractor
    {
        private const string RatesRequest =
            "https://www.priorbank.by/main?p_p_id=ExchangeRates_INSTANCE_ExchangeRatesCalculatorView&p_p_lifecycle=2&p_p_state=normal&p_p_mode=view&p_p_resource_id=ajaxSideBarConverterGetRates&p_p_cacheability=cacheLevelPage";


        public async Task<KomBankRates> GetRatesLineAsync()
        {
            var mainPage = await new WebExtractorAsync().GetPageAsync(RatesRequest, "utf-8", Encoding.UTF8);
            if (mainPage == "")
                return new KomBankRates { Bank = "Приор", StartedFrom = "error" };

            try
            {
                var json = mainPage.Replace("\"{", "{").Replace("}\"", "}").Replace(@"\", "");
                return Parse(json);
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} in Prior parser");
                return new KomBankRates { Bank = "Приор", StartedFrom = "error" };
            }
        }

        private KomBankRates Parse(string page)
        {
            var result = new KomBankRates { Bank = "Приор" };

            var priorList = (RootObject)JsonConvert.DeserializeObject(page, typeof(RootObject));
            if (priorList.Message != "success")
            {
                result.StartedFrom = "error";
                return result;
            }


            var onlineRate = priorList.CalculatorTable.Data[31];
            result.UsdA = onlineRate.Rate.BuyRate;
            result.UsdB = onlineRate.Rate.SellRate;

            DateTime date = onlineRate.ValidFrom;
            result.StartedFrom = $"{date}";
            result.LastCheck = DateTime.Now;

            onlineRate = priorList.CalculatorTable.Data[32];
            result.EurA = onlineRate.Rate.BuyRate;
            result.EurB = onlineRate.Rate.SellRate;
            onlineRate = priorList.CalculatorTable.Data[30];
            result.RubA = onlineRate.Rate.BuyRate;
            result.RubB = onlineRate.Rate.SellRate;

            onlineRate = priorList.CalculatorTable.Data[33];
            result.RubUsdA = onlineRate.Rate.BuyRate;
            result.RubUsdB = onlineRate.Rate.SellRate;
            onlineRate = priorList.CalculatorTable.Data[35];
            result.EurUsdA = onlineRate.Rate.BuyRate;
            result.EurUsdB = onlineRate.Rate.SellRate;
            onlineRate = priorList.CalculatorTable.Data[34];
            result.RubEurA = onlineRate.Rate.BuyRate;
            result.RubEurB = onlineRate.Rate.SellRate;

            return result;
        }


    }
}