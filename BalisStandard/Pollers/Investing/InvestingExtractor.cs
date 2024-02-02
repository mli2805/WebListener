using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace BalisStandard
{
    public class InvestingExtractor
    {
        private const string BaseUrl = "https://ru.investing.com/";

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
                var endIndex = page.IndexOf("<", index + 34, StringComparison.Ordinal);

                var sub = page.Substring(index + 34, endIndex - index - 34);
                sub = sub.Replace(".", "");
                if (double.TryParse(sub, NumberStyles.Any, null, out double rate))
                    return rate;
                else Console.WriteLine($"can't parse {sub}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return 0;
        }
    }
}
