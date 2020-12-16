using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace BalisStandard
{
    public class DabrabytExtractor : IRatesLineExtractor
    {
        public string BankTitle => KomBankE.Mmb.ToString().ToUpper();
        private const string Url = @"https://bankdabrabyt.by/currency_exchange/";

        private const string Nal = "НАЛИЧНЫЕ";
        private const string Conversion = "КОНВЕРСИЯ";
        private const string Card = "КАРТОЧКИ\"><div";

        public async Task<KomBankRatesLine> GetRatesLineAsync()
        {
            var page = await ((HttpWebRequest) WebRequest.Create(Url))
                .InitializeForKombanks()
                .GetDataAsync(); 
            if (string.IsNullOrEmpty(page))
                return null;

            return ParseThreeTables(page);
        }

        private KomBankRatesLine ParseThreeTables(string page)
        {
            var result = new KomBankRatesLine { Bank = BankTitle };

            if (!FetchTable(page, Nal, out string table))
                return null;
            if (!GetRates(table, "Российские рубли", out double kursA, out double kursB))
                return null;
            result.RubA = kursA;
            result.RubB = kursB;

            if (!FetchTable(page, Card, out table))
                return null;
            if (!GetRates(table, "Доллар США", out kursA, out kursB))
                return null;
            result.UsdA = kursA;
            result.UsdB = kursB;
            if (!GetRates(table, "Eвро", out kursA, out kursB))
                return null;
            result.EurA = kursA;
            result.EurB = kursB;


            if (!FetchTable(page, Conversion, out table))
                return null;
            if (!GetOneRate(table, "Доллар США/Eвро", out kursA))
                return null;
            result.EurUsdB = kursA;
            if (!GetOneRate(table, "Eвро/Доллар США", out kursB))
                return null;
            result.EurUsdA = kursB;

            if (!GetOneRate(table, "Доллар США/Российские рубли", out kursA))
                return null;
            result.RubUsdA = kursA;
            if (!GetOneRate(table, "Российские рубли/Доллар США", out kursB))
                return null;
            result.RubUsdB = kursB;


            if (!GetOneRate(table, "Eвро/Российские рубли", out kursA))
                return null;
            result.RubEurA = kursA;
            if (!GetOneRate(table, "Российские рубли/Eвро", out kursB))
                return null;
            result.RubEurB = kursB;

            result.StartedFrom = DateTime.Now;
            result.LastCheck = DateTime.Now;
            return result;
        }

        private bool FetchTable(string webData, string tableName, out string table)
        {
            table = "";
            try
            {
                var pos = webData.IndexOf(tableName, StringComparison.Ordinal);
                if (pos == -1) return false;
                var endTablePos = webData.IndexOf("/table", pos, StringComparison.Ordinal);
                table = webData.Substring(pos, endTablePos - pos);
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private bool GetOneRate(string table, string currencyKey, out double kurs)
        {
            kurs = 0;
            var pos = table.IndexOf(currencyKey, StringComparison.Ordinal);
            if (pos == -1) return false;
            pos = table.IndexOf("<td>", pos, StringComparison.Ordinal);

            var posFrom = pos + 4;
            var posTo = table.IndexOf("</td>", posFrom, StringComparison.Ordinal);
            var fragment = table.Substring(posFrom, posTo - posFrom);

            return double.TryParse(fragment, NumberStyles.Any, new CultureInfo("en-US"), out kurs);
        } 
        
        private bool GetRates(string table, string currencyKey, out double kursA, out double kursB)
        {
            kursA = 0;
            kursB = 0;
            // only RUB
            var pos = table.IndexOf(currencyKey, StringComparison.Ordinal);
            if (pos == -1) return false;
            pos = table.IndexOf("<td>", pos, StringComparison.Ordinal);

            var posFrom = pos + 4;
            var posTo = table.IndexOf("</td>", posFrom, StringComparison.Ordinal);
            var fragment = table.Substring(posFrom, posTo - posFrom);

            if (!double.TryParse(fragment, NumberStyles.Any, new CultureInfo("en-US"), out kursA))
                return false;

            pos = table.IndexOf("<td>", posTo, StringComparison.Ordinal);
            posFrom = pos + 4;
            posTo = table.IndexOf("</td>", posFrom, StringComparison.Ordinal);
            fragment = table.Substring(posFrom, posTo - posFrom);

            return double.TryParse(fragment, NumberStyles.Any, new CultureInfo("en-US"), out kursB);
        }
    }
}
