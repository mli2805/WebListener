using System;
using System.Net;
using System.Threading.Tasks;

namespace BalisStandard
{
    public class Banki24ArchiveExtractor
    {
        /// <summary>
        /// example "http://banki24.by/exchange/currencymarket/RUB/2016-03-03"
        /// </summary>
        /// <param name="date"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public async Task<BelStockArchiveOneCurrency> GetOneCurrencyDayAsync(DateTime date, Currency currency)
        {
            var url = "http://banki24.by/exchange/currencymarket/" + currency.ToString().ToUpper() + "/" + date.ToString("yyyy-MM-dd");

            var page = await ((HttpWebRequest)WebRequest.Create(url))
                .InitializeForKombanks()
                .GetDataAsync();
            if (string.IsNullOrEmpty(page)) return null;

            return ParseOneCurrencyTable(page, currency, date);
        }

        private BelStockArchiveOneCurrency ParseOneCurrencyTable(string webpage, Currency currency, DateTime date)
        {
            try
            {
                var avpos = webpage.IndexOf("<h3 class=\"h2\">Средневзвешенный курс: <span class=\"text-bold\">",
                    StringComparison.Ordinal);
                var avposFrom = avpos + 62;
                var avposTo = webpage.IndexOf("</span>", avposFrom, StringComparison.Ordinal);
                var aver = webpage.Substring(avposFrom, avposTo - avposFrom);
                var averRate = aver.ParseDoubleFromWebTrash();

                var pos = webpage.IndexOf("<h3>Характеристики торгов", StringComparison.Ordinal);
                var posFrom = webpage.IndexOf("table", pos, StringComparison.Ordinal);
                var posTo = webpage.IndexOf("/table", posFrom, StringComparison.Ordinal);

                var table = webpage.Substring(posFrom, posTo - posFrom);
                if (string.IsNullOrEmpty(table))
                    return null;

                return new BelStockArchiveOneCurrency
                {
                    Date = date,
                    Currency = currency,
                    Average = averRate,
                    First = ParseValue(table, "Курс открытия"),
                    Min = ParseValue(table, "Минимальный курс"),
                    Max = ParseValue(table, "Максимальный курс"),
                    Last = ParseValue(table, "Курс последней сделки"),
                    TurnoverInCurrency = ParseValue(table, $"Оборот в {currency.ToString().ToUpper()}, млн."),
                    TurnoverInByn = ParseValue(table, "Оборот в BYN, млн."),
                    TurnoverInUsd = ParseValue(table, "Оборот в USD, млн."),
                    Count = (int)ParseValue(table, "Количество сделок")
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static double ParseValue(string table, string param)
        {
            try
            {
                var pos = table.IndexOf(param, StringComparison.Ordinal);
                var posFrom = table.IndexOf("<td>", pos, StringComparison.Ordinal) + 4;
                var posTo = table.IndexOf("</td>", posFrom, StringComparison.Ordinal);
                var valueString = table.Substring(posFrom, posTo - posFrom).Trim();
                return valueString.ParseDoubleFromWebTrash();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }
    }
}
