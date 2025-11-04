using System;
using System.Globalization;
using HtmlAgilityPack;

namespace BalisStandard;

public class PriorFullPageParser : IFullPageParser
{
    public KomBankRatesLine? ParseKomBankRatesFromHtml(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var result = new KomBankRatesLine
        {
            Bank = KomBankE.Prior.ToString().ToUpper(),
            LastCheck = DateTime.Now,
            StartedFrom = ExtractStartedFrom(doc) ?? DateTime.Now
        };

        var rows = doc.DocumentNode.SelectNodes("//div[contains(@class,'currency-rates__row-body')]");
        if (rows == null) return null;

        foreach (var row in rows)
        {
            var cells = row.SelectNodes(".//div[contains(@class,'currency-rates__data')]");
            if (cells == null || cells.Count < 3) continue;

            var currency = NormalizeCurrency(cells[0]);
            var buy = ParseRate(cells[3]);
            var sell = ParseRate(cells[4]);

            switch (currency)
            {
                case "1USD":
                    result.UsdA = sell;
                    result.UsdB = buy;
                    break;
                case "1EUR":
                    result.EurA = sell;
                    result.EurB = buy;
                    break;
                case "100RUB":
                    result.RubA = sell;
                    result.RubB = buy;
                    break;
                case "10CNY":
                    result.CnyA = sell;
                    result.CnyB = buy;
                    break;
                case "EUR/USD":
                    result.EurUsdA = sell;
                    result.EurUsdB = buy;
                    break;
                case "USD/RUB":
                    result.RubUsdA = sell;
                    result.RubUsdB = buy;
                    break;
                case "EUR/RUB":
                    result.RubEurA = sell;
                    result.RubEurB = buy;
                    break;
                case "USD/CNY":
                    result.UsdCnyA = sell;
                    result.UsdCnyB = buy;
                    break;
                case "EUR/CNY":
                    result.EurCnyA = sell;
                    result.EurCnyB = buy;
                    break;
                case "CNY/RUB":
                    result.CnyRubA = sell;
                    result.CnyRubB = buy;
                    break;
            }
        }

        return result;
    }

    private static string NormalizeCurrency(HtmlNode cell)
    {
        var textNode = cell.SelectSingleNode(".//span[contains(@class,'currency-rates__data-title-text')]");
        if (textNode == null) return "";

        var raw = textNode.InnerText.Trim().Replace("\n", "").Replace("\t", "").Replace(" ", "");
        return raw;
    }

    private static double ParseRate(HtmlNode cell)
    {
        var span = cell.SelectSingleNode(".//span[contains(@class,'currency-rates__data-val-text')]");
        if (span == null) return 0;
        var text = span.InnerText.Trim().Replace(",", ".");
        return double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var value) ? value : 0;
    }

    private static DateTime? ExtractStartedFrom(HtmlDocument doc)
    {
        var timeNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'currency-rates__date')]");
        if (timeNode == null) return null;

        var timeText = timeNode.SelectSingleNode(".//span[contains(@class,'currency-rates__date-time')]")?.InnerText?.Trim();
        var dateText = timeNode.SelectSingleNode(".//span[contains(@class,'currency-rates__date-day')]")?.InnerText?.Trim();

        if (string.IsNullOrEmpty(timeText) || string.IsNullOrEmpty(dateText)) return null;

        var fullText = $"{dateText} {timeText}";
        return DateTime.TryParseExact(fullText, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
            ? dt
            : null;
    }
}