using BalisStandard;
using System;
using System.Net;
using System.Threading.Tasks;
using LoadPlaywright;
using UtilsLib;

namespace BalisWpf
{
    public class BelStockExtractorAndParser
    {
        private readonly IMyLog _logFile;
        private readonly string _playwrightPath;
        private const string Url = "https://banki24.by/exchange/currencymarket";
        private const string UrlUsd = "https://banki24.by/exchange/currencymarket/USD";
        private const string UrlEur = "https://banki24.by/exchange/currencymarket/EUR";
        private const string UrlRub = "https://banki24.by/exchange/currencymarket/RUB";
        private const string UrlCny = "https://banki24.by/exchange/currencymarket/CNY";

        public BelStockExtractorAndParser(IMyLog logFile, string playwrightPath)
        {
            _logFile = logFile;
            _playwrightPath = playwrightPath;
        }

        public async Task<BelStock> GetStockAsync()
        {
            var extractor = new PlaywrightExtractor(_logFile, _playwrightPath, false);
            try
            {
                var response = await extractor.Fetch(Url);
                var belStock = Parse(response);
                if (belStock == null)
                    return null;
                if (belStock.Usd.Average > -1)
                {
                    var usdPage = await extractor.Fetch(UrlUsd);
                    belStock.Usd.LastDeal = ParseLastDealRate(usdPage);
                    belStock.Usd.DealsCount = ParseDealsCount(usdPage);
                }

                if (belStock.Eur.Average > -1)
                {
                    var eurPage = await extractor.Fetch(UrlEur);
                    belStock.Eur.LastDeal = ParseLastDealRate(eurPage);
                    belStock.Eur.DealsCount = ParseDealsCount(eurPage);
                }

                if (belStock.Rub.Average > -1)
                {
                    var rubPage =
                        await extractor.Fetch(UrlRub);
                    belStock.Rub.LastDeal = ParseLastDealRate(rubPage);
                    belStock.Rub.DealsCount = ParseDealsCount(rubPage);
                }

                if (belStock.Cny.Average > -1)
                {
                    var cnyPage = await extractor.Fetch(UrlCny);
                    belStock.Cny.LastDeal = ParseLastDealRate(cnyPage);
                    belStock.Cny.DealsCount = ParseDealsCount(cnyPage);
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
            var rate = rateString.ParseDoubleFromWebTrash();
            return rate;
        }
        private double ParseDealsCount(string webData)
        {
            var pos = webData.IndexOf("Количество сделок", StringComparison.Ordinal);
            var posFrom = webData.IndexOf("<td>", pos + 2, StringComparison.Ordinal);
            var posTo = webData.IndexOf("</td>", posFrom + 2, StringComparison.Ordinal);
            var rateString = webData.Substring(posFrom + 5, posTo - posFrom - 1);
            var rate = rateString.ParseDoubleFromWebTrash();
            return rate;
        }

        private BelStock Parse(string webData)
        {
            var table = webData;
            if (table == "") return null;
            var result = new BelStock
            {
                LastChecked = DateTime.Now,
                TradingState = GetState(table, out var pos),
                TradingDate = GetTradingDate(table, pos)
            };
            if (result.TradingDate == new DateTime(1900, 1, 1))
            {
                result.Usd.Average = -1;
                result.Eur.Average = -1;
                result.Rub.Average = -1;
                result.Cny.Average = -1;
                return result;
            }
            double rate;
            string volume;
            GetForCurrency(table, "USD", out rate, out volume);
            result.Usd.Average = rate;
            result.Usd.Volume = volume;

            GetForCurrency(table, "EUR", out rate, out volume);
            result.Eur.Average = rate;
            // if (volume.Length > 7) 
                result.Eur.Volume = volume;

            GetForCurrency(table, "RUB", out rate, out volume);
            result.Rub.Average = rate;
            // if (volume.Length > 7) 
                result.Rub.Volume = volume;

            GetForCurrency(table, "CNY", out rate, out volume);
            result.Cny.Average = rate;
            // if (volume.Length > 7) 
                result.Cny.Volume = volume;

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
            return DateTime.TryParse(dateString, out var result) 
                ? result 
                : new DateTime(1900, 1, 1);
        }
        private void GetForCurrency(string table, string currency, out double rate, out string volume)
        {
            var key = $"<a href=\"/exchange/currencymarket/{currency.ToLower()}\">{currency}</a>";
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
            rate = rateString.ParseDoubleFromWebTrash();

            pos = table.IndexOf(">Объём, млн. USD</span>", posTo, StringComparison.Ordinal);
            posFrom = table.IndexOf(">", pos + 27, StringComparison.Ordinal);
            posTo = table.IndexOf("</span", posFrom, StringComparison.Ordinal);
            volume = table.Substring(posFrom + 1, posTo - posFrom - 1).Trim();
        }
        
    }
}
