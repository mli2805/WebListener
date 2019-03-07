using System;
using Extractors;

namespace WebListener
{
    public class Banki24ArchivePageParser
    {
        public string FetchTable(string webpage)
        {
            try
            {
                var pos = webpage.IndexOf("<h3>Характеристики торгов</h3>", StringComparison.Ordinal);
                var posFrom = webpage.IndexOf("table", pos, StringComparison.Ordinal);
                var posTo = webpage.IndexOf("/table", posFrom, StringComparison.Ordinal);

                return webpage.Substring(posFrom, posTo - posFrom);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public BelStockArchiveOneCurrencyDay ParseOneCurrency(string table, Currency currency)
        {
            return new BelStockArchiveOneCurrencyDay
            {
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