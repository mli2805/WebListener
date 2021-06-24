using System;
using System.Net;
using System.Threading.Tasks;

namespace BalisStandard
{
    public class Banki24Extractor
    {
        private const string Url = "http://banki24.by/exchange/currencymarket";
        private const string UrlUsd = "http://banki24.by/exchange/currencymarket/USD";
        private const string UrlEur = "http://banki24.by/exchange/currencymarket/EUR";
        private const string UrlRub = "http://banki24.by/exchange/currencymarket/RUB";

        public async Task<BelStock> GetStockAsync()
        {
            try
            {
                var belStock = Parse(await ((HttpWebRequest)WebRequest.Create(Url))
                    .InitializeForKombanks()
                    .GetDataAsync());
                if (belStock == null)
                    return null;
                if (belStock.Usd.Average > -1)
                {
                    var usdPage = await ((HttpWebRequest)WebRequest.Create(UrlUsd))
                        .InitializeForKombanks()
                        .GetDataAsync();
                    belStock.Usd.LastDeal = ParseLastDealRate(usdPage);
                    belStock.Usd.DealsCount = ParseDealsCount(usdPage);
                }

                if (belStock.Eur.Average > -1)
                {
                    var eurPage = await ((HttpWebRequest)WebRequest.Create(UrlEur))
                        .InitializeForKombanks()
                        .GetDataAsync();
                    belStock.Eur.LastDeal = ParseLastDealRate(eurPage);
                    belStock.Eur.DealsCount = ParseDealsCount(eurPage);
                }

                if (belStock.Rub.Average > -1)
                {
                    var rubPage = await ((HttpWebRequest)WebRequest.Create(UrlRub))
                        .InitializeForKombanks()
                        .GetDataAsync();
                    belStock.Rub.LastDeal = ParseLastDealRate(rubPage);
                    belStock.Rub.DealsCount = ParseDealsCount(rubPage);
                }

                return belStock;
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} in Bel Stock online parser");
                return null;
            }
        }

        private double ParseLastDealRate(string webData)
        {
            var pos = webData.IndexOf("Курс последней сделки", StringComparison.Ordinal);
            var posFrom = webData.IndexOf("<td>", pos + 2, StringComparison.Ordinal);
            var posTo = webData.IndexOf("</td>", posFrom + 2, StringComparison.Ordinal);
            var rateString = webData.Substring(posFrom + 5, posTo - posFrom - 1);
            var rate = SpecialOperations.ParseDoubleFromWebTrash(rateString);
            return rate;
        }
        private double ParseDealsCount(string webData)
        {
            var pos = webData.IndexOf("Количество сделок", StringComparison.Ordinal);
            var posFrom = webData.IndexOf("<td>", pos + 2, StringComparison.Ordinal);
            var posTo = webData.IndexOf("</td>", posFrom + 2, StringComparison.Ordinal);
            var rateString = webData.Substring(posFrom + 5, posTo - posFrom - 1);
            var rate = SpecialOperations.ParseDoubleFromWebTrash(rateString);
            return rate;
        }

        private BelStock Parse(string webData)
        {
            var table = FetchTable(webData);
            if (table == "") return null;
            int pos;
            var result = new BelStock
            {
                LastChecked = DateTime.Now,
                TradingState = GetState(table, out pos),
                TradingDate = GetTradingDate(table, pos)
            };
            if (result.TradingDate == new DateTime(1900, 1, 1))
            {
                result.Usd.Average = -1;
                result.Eur.Average = -1;
                result.Rub.Average = -1;
                return result;
            }
            double rate;
            string volume;
            GetForCurrency(table, "USD", out rate, out volume);
            result.Usd.Average = rate;
            result.Usd.Volume = volume;
            GetForCurrency(table, "EUR", out rate, out volume);
            result.Eur.Average = rate;
            if (volume.Length > 7) result.Eur.Volume = volume.Substring(7);
            GetForCurrency(table, "RUB", out rate, out volume);
            result.Rub.Average = rate;
            if (volume.Length > 7) result.Rub.Volume = volume.Substring(7);
            return result;
        }

        /// <summary>
        /// до 10-00 либо в выходной <span class="label label-warning">Результаты торгов за 18.12.2015</span>
        /// с 10-00 <span class="label label-success">on-line торги сегодня 18.12.2015</span>
        /// после торгов <span class="label label-info">Результаты торгов за сегодня 18.12.2015</span>
        /// </summary>
        /// <param name="table"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private BelStockState GetState(string table, out int pos)
        {
            pos = table.IndexOf("<span class=\"label label-warning\">Результаты", StringComparison.Ordinal);
            if (pos != -1)
                return BelStockState.HasNotStartedYet;
            pos = table.IndexOf("<span class=\"label label-success\">on-line", StringComparison.Ordinal);
            if (pos != -1)
            {
                if (DateTime.Now < DateTime.Today.AddMinutes(600)) return BelStockState.HasNotStartedYet;  // до 10-00 считаем торги не начавшимися
                if (DateTime.Now > DateTime.Today.AddMinutes(800)) return BelStockState.TerminatedAlready; // после 13-20 считаем торги закончившимися
                return BelStockState.InProgress;
            }
            pos = table.IndexOf("<span class=\"label label-info\">Результаты", StringComparison.Ordinal);
            if (pos != -1)
                return BelStockState.TerminatedAlready;
            return BelStockState.FetchingError;
        }


        private DateTime GetTradingDate(string table, int startIndex)
        {
            var pos = table.IndexOf("</span", startIndex, StringComparison.Ordinal);
            var dateString = table.Substring(pos - 10, 10);
            DateTime result;
            if (DateTime.TryParse(dateString, out result)) return result; else return new DateTime(1900, 1, 1);
        }
        private void GetForCurrency(string table, string currency, out double rate, out string volume)
        {
            var key = string.Format("<a href=\"/exchange/currencymarket/{0}\">{1}</a>", currency.ToLower(), currency);
            var pos = table.IndexOf(key, StringComparison.Ordinal);
            pos = table.IndexOf("<p class=\"text-center h1 mt-0\">", pos + 5, StringComparison.Ordinal);
            var posFrom = pos + 29;
            var posTo = table.IndexOf("<span", posFrom, StringComparison.Ordinal);
            if (posTo - posFrom - 2 < 0)
            {
                rate = -1;
                volume = "";
                return;
            }
            var rateString = table.Substring(posFrom, posTo - posFrom - 2);
            rate = SpecialOperations.ParseDoubleFromWebTrash(rateString);

            pos = table.IndexOf(">Объём, млн. USD</span>", posTo, StringComparison.Ordinal);
            posFrom = table.IndexOf(">", pos + 27, StringComparison.Ordinal);
            posTo = table.IndexOf("</span", posFrom, StringComparison.Ordinal);
            volume = table.Substring(posFrom + 1, posTo - posFrom - 1);
        }
        private string FetchTable(string webData)
        {
            var pos = webData.IndexOf("<h1>Торги на белорусской валютно-фондовой бирже</h1>", StringComparison.Ordinal);
            if (pos == -1) return "";
            var endTablePos = webData.IndexOf("<h3>Архив валютных торгов</h3>", StringComparison.Ordinal);
            return webData.Substring(pos, endTablePos - pos);
        }

    }
}
