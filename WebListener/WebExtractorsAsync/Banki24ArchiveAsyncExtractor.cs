using System;
using System.Text;
using System.Threading.Tasks;
using WebListener.DomainModel;
using WebListener.DomainModel.BelStock;
using WebListener.Parsers;

namespace WebListener.WebExtractorsAsync
{
    public class Banki24ArchiveAsyncExtractor
    {
        public async Task<BelStockArchiveDay> GetBelStockDayAsync(DateTime date)
        {
            var result = new BelStockArchiveDay();

            result.Usd = await GetOneCurrencyResultAsync(date, Currency.Usd);
            if (result.Usd == null) return null;
            result.Eur = await GetOneCurrencyResultAsync(date, Currency.Eur);
            result.Rub = await GetOneCurrencyResultAsync(date, Currency.Rub);

            result.Date = date;
            return result;
        }

        /// <summary>
        /// example "http://banki24.by/exchange/currencymarket/RUB/2016-03-03"
        /// </summary>
        /// <param name="date"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        private async Task<BelStockArchiveOneCurrencyDay> GetOneCurrencyResultAsync(DateTime date, Currency currency)
        {
            var url = "http://banki24.by/exchange/currencymarket/" + currency.ToString().ToUpper() + "/" + date.ToString("yyyy-MM-dd");

            var page = await new WebExtractor().GetPageAsync(url, "utf-8", Encoding.UTF8);
            if (page == "") return null;

            var parser = new Banki24ArchivePageParser();
            var table = parser.FetchTable(page);
            return table == "" ? null : parser.ParseOneCurrency(table, currency);
        }

    }
}
