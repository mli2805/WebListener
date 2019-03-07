using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebListener
{
    public class PriorExtractor
    {
        private DateTime _startDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private const string RatesRequest =
            "https://www.priorbank.by/?p_p_id=exchangeliferayspringmvcportlet_WAR_exchangeliferayspringmvcportlet&p_p_lifecycle=2&p_p_state=normal&p_p_mode=view&p_p_resource_id=ajaxGetReportForMainPageAjax&p_p_cacheability=cacheLevelPage&p_p_col_id=_118_INSTANCE_7La20uxMthb5__column-2&p_p_col_count=1";
        public async Task<KomBankRates> GetRatesLineAsync()
        {
            var mainPage = await new WebExtractor().GetPageAsync(RatesRequest, "utf-8", Encoding.UTF8);
            if (mainPage == "")
                return new KomBankRates { Bank = "Приор", StartedFrom = "error" };

            try
            {
                return Parse(mainPage);
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
            if (priorList.Message != "Success")
            {
                result.StartedFrom = "error";
                return result;
            }


            var onlineRates = priorList.FullListAjax.Where(r => r.ChannelId == 20).ToList();
            var onlineUsd = onlineRates.First(r => r.CurrencyType == "USD").ViewVoList[0];
            result.UsdA = onlineUsd.Buy;
            result.UsdB = onlineUsd.Sell;

            DateTime date = _startDate.AddMilliseconds(onlineUsd.AriseTime).ToLocalTime();
            result.StartedFrom = $"{date}";
            result.LastCheck = DateTime.Now;

            var onlineEur = onlineRates.First(r => r.CurrencyType == "EUR").ViewVoList[0];
            result.EurA = onlineEur.Buy;
            result.EurB = onlineEur.Sell;
            var onlineRub = onlineRates.First(r => r.CurrencyType == "RUB").ViewVoList[0];
            result.RubA = onlineRub.Buy;
            result.RubB = onlineRub.Sell;

            var onlineConvertions = priorList.FullListAjax.Where(r => r.ChannelId == 21).ToList();
            var onlineRubUsd = onlineConvertions.First(r => r.CurrencyType == "USD/RUB").ViewVoList[0];
            result.RubUsdA = onlineRubUsd.Buy;
            result.RubUsdB = onlineRubUsd.Sell;
            var onlineEurUsd = onlineConvertions.First(r => r.CurrencyType == "EUR/USD").ViewVoList[0];
            result.EurUsdA = onlineEurUsd.Buy;
            result.EurUsdB = onlineEurUsd.Sell;
            var onlineRubEur = onlineConvertions.First(r => r.CurrencyType == "EUR/RUB").ViewVoList[0];
            result.RubEurA = onlineRubEur.Buy;
            result.RubEurB = onlineRubEur.Sell;

            return result;
        }


    }
}