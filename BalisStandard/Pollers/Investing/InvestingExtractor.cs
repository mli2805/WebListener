using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace BalisStandard
{
    public class InvestingExtractor
    {
        private const string Url = "https://ru.investing.com/currencies/usd-rub";

        public async Task<double> GetRate()
        {
            try
            {
                var page = await ((HttpWebRequest)WebRequest.Create(Url)).GetDataAsync();
                var index = page.IndexOf("data-test=\"instrument-price-last\">", StringComparison.Ordinal);
                var sub = page.Substring(index + 34, 7);
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
