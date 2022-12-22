using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace BalisStandard
{
    public class InvestingExtractor
    {
        private const string BaseUrl = "https://ru.investing.com/currencies/";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currencyPair"> usd-rub, eur-rub, cny-rub </param>
        /// <returns></returns>
        public async Task<double> GetRate(string currencyPair)
        {
            try
            {
                var uri = BaseUrl + currencyPair;
                var page = await ((HttpWebRequest)WebRequest.Create(uri)).GetDataAsync();
                var index = page.IndexOf("data-test=\"instrument-price-last\">", StringComparison.Ordinal);
                var sub = page.Substring(index + 34, 6);
                if (double.TryParse(sub, NumberStyles.Any, null, out double rate))
                    return rate;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return 0;
        }
    }
}
