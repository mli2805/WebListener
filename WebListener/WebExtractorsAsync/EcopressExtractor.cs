using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using WebListener.DomainModel;

namespace WebListener.WebExtractorsAsync
{
    public class EcopressExtractor
    {
        private const string PageName = "http://www.ecopress.by/ru/page/65.html";
        private static readonly string[] Banks = {"Белгазпромбанк", "Банк Москва–Минск", "Белинвестбанк", "БПС–Сбербанк" };

        public async Task<EcopressRates> GetRatesAsync()
        {
            var page = await new WebExtractor().GetPageAsync(PageName, "Windows-1251", Encoding.Default);
            if (page == "") return null;

            try
            {
                return ParsePage(page);
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} in EcopressExtractor");
                return null;
            }
        }

        private KomBankRates ParseLine(string line)
        {
            if (line == "") return null;

            var result = new KomBankRates();

            int posTo = 0;
            result.LastCheck = GetTime(line, ref posTo);

            double rateA, rateB;
            GetRatesPair(line, ref posTo, out rateA, out rateB); // usd
            result.UsdA = rateA; 
            result.UsdB = rateB;
            GetRatesPair(line, ref posTo, out rateA, out rateB); // eur
            result.EurA = rateA;
            result.EurB = rateB;
            GetRatesPair(line, ref posTo, out rateA, out rateB); // rur
            result.RubA = rateA;
            result.RubB = rateB;

            return result;
        }

        private static string GetRidOfStrong(string str)
        {
            var pos = str.IndexOf("<strong>", StringComparison.Ordinal);
            if (pos == -1) return str;
            var posTo = str.IndexOf("</strong>", StringComparison.Ordinal);
            return str.Substring(pos + 8, posTo - (pos + 8));
        }

        private static void GetRatesPair(string line, ref int posTo, out double rateA, out double rateB)
        {
            rateA = -1;
            rateB = -1;
            var pos = line.IndexOf("dark", posTo, StringComparison.Ordinal);
            if (pos == -1) return;
            posTo = line.IndexOf("<td", pos, StringComparison.Ordinal);
            var rateStr = line.Substring(pos + 6, posTo - (pos + 6));
            rateStr = GetRidOfStrong(rateStr);
            if (!Double.TryParse(rateStr, NumberStyles.Any, new CultureInfo("en-US"), out rateA)) rateA = -1;

            pos = posTo;
            posTo = line.IndexOf("<td", pos+1, StringComparison.Ordinal);
            rateStr = line.Substring(pos + 4, posTo - (pos + 4));
            rateStr = GetRidOfStrong(rateStr);
            if (!Double.TryParse(rateStr, NumberStyles.Any, new CultureInfo("en-US"), out rateB)) rateB = -1;
        }

        private static DateTime GetTime(string line, ref int posTo)
        {
            var pos = line.IndexOf("time", posTo, StringComparison.Ordinal);
            if (pos == -1) return DateTime.MinValue;

            posTo = line.IndexOf("</td>", pos, StringComparison.Ordinal);
            var timeStr = line.Substring(pos + 6, posTo-(pos+6));
            return Convert.ToDateTime(timeStr);
        }

        private string GetLine(string page, string bank)
        {
            var pos = page.IndexOf(bank, StringComparison.Ordinal);
            if (pos == -1) return "";
            var posTo = page.IndexOf("</tr>", pos, StringComparison.Ordinal);
            var line = page.Substring(pos, posTo - (pos + 6));
            return line;
        }

        private EcopressRates ParsePage(string webData)
        {
            var result = new EcopressRates();
            foreach (var bank in Banks)
            {
                var line = ParseLine(GetLine(webData, bank));
                if (line != null)
                {
                    line.Bank = GetBank(bank).ToString();
                    line.StartedFrom = DateTime.Now.ToString(CultureInfo.CurrentCulture);
                    result.List.Add(line);
                }
            }
            return result;
        }

        private KomBank GetBank(string name)
        {
            if (name == "Белгазпромбанк") return KomBank.БГПБ;
            if (name == "Банк Москва–Минск") return KomBank.ММБ;
            if (name == "Белинвестбанк") return KomBank.БИБ;
            if (name == "БПС–Сбербанк") return KomBank.БПС;
            return 0;
        }
    }
}
