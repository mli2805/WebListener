using HtmlAgilityPack;
using System;
using System.Globalization;
using System.Linq;

namespace BalisStandard;

public class BelStockFullPageParser
{
    /// <summary>
    /// незаконченный парсер !!!!!!!!!!!!!!!!!!!!!!
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public BelStock? Parse(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var result = new BelStock();
        result.LastChecked = DateTime.Now;

        // Дата и статус торгов
        ParseDateAndStatus(doc, ref result);

        // Парсим блоки валют
        var blocks = doc.DocumentNode.SelectNodes("//div[@class='info-block']");
        if (blocks == null)
            // throw new Exception("Не найдены валютные блоки на странице");
            return null;

        foreach (var block in blocks)
        {
            var nameNode = block.SelectSingleNode(".//h2//a");
            if (nameNode == null) continue;

            var code = nameNode.InnerText.Trim().ToUpperInvariant();
            var avgText = block.SelectSingleNode(".//p[@class='text-center h1 mt-0']")
                ?.InnerText?.Trim().Replace(',', '.');
            var absText = block.SelectSingleNode(".//span[contains(@class,'label') and contains(text(),'−')]")
                ?.InnerText?.Replace("−", "-").Replace("\n", "").Replace("\r", "")
                .Replace("\t", "").Trim().Replace(',', '.');
            var relText = block.SelectSingleNode(".//span[contains(@class,'label') and contains(text(),'%')]")
                ?.InnerText?.Replace("−", "-").Replace("\n", "").Replace("\r", "")
                .Replace("\t", "").Replace("%", "").Trim().Replace(',', '.');

            var volume = code == "USD"
                    ? block.SelectSingleNode(".//p[span[contains(text(),'Объём')]]/span[@class='pull-right']")?.InnerText?.Trim()
                : block.SelectSingleNode(".//p/span[contains(text(),'≈')]")?.InnerText?.Trim();



            var currency = new BelStockCurrency();
            if (double.TryParse(avgText, NumberStyles.Any, CultureInfo.InvariantCulture, out var avg))
                currency.Average = avg;
            if (double.TryParse(absText, NumberStyles.Any, CultureInfo.InvariantCulture, out var abs))
                currency.AbsoluteChanges = abs;
            if (double.TryParse(relText, NumberStyles.Any, CultureInfo.InvariantCulture, out var rel))
                currency.RelativeChanges = rel;
            currency.Volume = volume;

            switch (code)
            {
                case "USD": result.Usd = currency; break;
                case "EUR": result.Eur = currency; break;
                case "RUB": result.Rub = currency; break;
                case "CNY": result.Cny = currency; break;
            }
        }

        return result;
    }

    void ParseDateAndStatus(HtmlDocument doc, ref BelStock belStock)
    {
        // XPath для поиска параграфа, за которым сразу следует div с классом exchange-container
        var xpath = "//p[following-sibling::*[1][self::div and contains(@class, 'exchange-container')]]";
        var p = doc.DocumentNode.SelectSingleNode(xpath);
        if (p == null) return;

        var spanText = p.SelectSingleNode(".//span")?.InnerText;
        if (spanText == null) return;

        int lastSpaceIndex = spanText.LastIndexOf(' ');

        var statusText = spanText.Substring(0, lastSpaceIndex).Trim();
        if (statusText.StartsWith("Результаты торгов за сегодня")) 
            belStock.TradingState = BelStockState.TerminatedAlready;
        else if (statusText.StartsWith("Результаты торгов"))
        {
            // но это вчерашние начались уже
            belStock.TradingState = BelStockState.TerminatedAlready;
        }
        else
        {
            belStock.TradingState = BelStockState.InProgress;
        }
       

        var dateText = spanText.Substring(lastSpaceIndex + 1).Trim();

        if (DateTime.TryParseExact(dateText, "dd.MM.yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            belStock.TradingDate = date;
        }
    }
}