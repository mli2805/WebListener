using System;
using System.Net;
using System.Threading.Tasks;

namespace BalisLibrary
{
    public class Banki24ArchiveExtractor
    {
        /// <summary>
        /// example "http://banki24.by/exchange/currencymarket/RUB/2016-03-03"
        /// </summary>
        /// <param name="date"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public async Task<BelStockArchiveOneCurrencyDay> GetOneCurrencyDayAsync(DateTime date, Currency currency)
        {
            var url = "http://banki24.by/exchange/currencymarket/" + currency.ToString().ToUpper() + "/" + date.ToString("yyyy-MM-dd");

            var page = await ((HttpWebRequest)WebRequest.Create(url))
                .InitializeForKombanks()
                .GetDataAsync();
            if (string.IsNullOrEmpty(page)) return null;

            return ParseOneCurrencyTable(page, currency, date);
        }

        private BelStockArchiveOneCurrencyDay ParseOneCurrencyTable(string webpage, Currency currency, DateTime date)
        {
            try
            {
                var pos = webpage.IndexOf("<h3>Характеристики торгов</h3>", StringComparison.Ordinal);
                var posFrom = webpage.IndexOf("table", pos, StringComparison.Ordinal);
                var posTo = webpage.IndexOf("/table", posFrom, StringComparison.Ordinal);

                var table = webpage.Substring(posFrom, posTo - posFrom);
                if (string.IsNullOrEmpty(table))
                    return null;

                return new BelStockArchiveOneCurrencyDay
                {
                    Date = date,
                    Currency = currency,
                    First = ParseValue(table, "Курс открытия"),
                    Min = ParseValue(table, "Минимальный курс"),
                    Max = ParseValue(table, "Максимальный курс"),
                    Last = ParseValue(table, "Курс последней сделки"),
                    TurnoverInCurrency = ParseValue(table, $"Оборот в {currency.ToString().ToUpper()}, млн."),
                    TurnoverInByn = ParseValue(table, "Оборот в BYN, млн."),
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
                return SpecialOperations.ParseDoubleFromWebTrash(valueString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }
    }
}
