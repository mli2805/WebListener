using System.Globalization;
using System;
using System.Linq;
using HtmlAgilityPack;

namespace BalisStandard;

public class AlfaFullPageParser : IFullPageParser
{
    public KomBankRatesLine? ParseKomBankRatesFromHtml(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var result = new KomBankRatesLine
        {
            Bank = KomBankE.Alfa.ToString().ToUpper(),
            LastCheck = DateTime.Now,
            StartedFrom = ExtractRatesStartTime(html) ?? DateTime.Now // fallback
        };

        var rows = doc.DocumentNode.SelectNodes("//table[contains(@class,'table__element')]/tr");
        if (rows == null) return null;

        foreach (var row in rows)
        {
            var cells = row.SelectNodes(".//td");
            if (cells == null || cells.Count < 3)
                continue;

            var currency = NormalizeCurrency(cells[0]);

            var sell = ParseRate(cells[1]);
            var buy = ParseRate(cells[2]);

            switch (currency)
            {
                case "USD":
                    result.UsdA = sell;
                    result.UsdB = buy;
                    break;
                case "EUR":
                    result.EurA = sell;
                    result.EurB = buy;
                    break;
                case "RUB":
                case "RUB 100":
                    result.RubA = sell;
                    result.RubB = buy;
                    break;
                case "CNY":
                case "CNY 10":
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
                default:
                    Console.WriteLine($"Неизвестная валюта: {currency}");
                    break;

            }
        }
        return result;
    }

    private static string NormalizeCurrency(HtmlNode cell)
    {
        var labelNode = cell.SelectSingleNode(".//div[contains(@class,'price__label')]");
        if (labelNode == null) return "";

        // Извлекаем весь текст из всех текстовых узлов внутри labelNode
        var raw = string.Concat(labelNode
                .DescendantsAndSelf()
                .Where(n => n.NodeType == HtmlNodeType.Text)
                .Select(n => n.InnerText))
            .Trim()
            .Replace("\n", "")
            .Replace("\t", "")
            .Replace(" ", "");

        // Нормализация RUB100 → RUB, CNY10 → CNY
        if (raw.StartsWith("RUB100")) return "RUB";
        if (raw.StartsWith("CNY10")) return "CNY";

        return raw;
    }

    private static double ParseRate(HtmlNode cell)
    {
        var span = cell.SelectSingleNode(".//span");
        if (span == null) return 0;
        var text = span.InnerText.Trim().Replace(",", ".");
        return double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var value) ? value : 0;
    }

    private static DateTime? ExtractRatesStartTime(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Ищем div, содержащий текст "Курс действует с"
        var timeNode = doc.DocumentNode.SelectSingleNode("//div[contains(text(),'Курс действует с')]");
        if (timeNode == null) return null;

        // Извлекаем текст, например: "Курс действует с 04.11.2025 08:30"
        var text = timeNode.InnerText.Trim();

        // Ищем дату и время через регулярное выражение
        var prefix = "Курс действует с ";
        var index = text.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
        if (index < 0) return null;

        var timePart = text.Substring(index + prefix.Length).Trim();

        // Преобразуем в DateTime
        if (DateTime.TryParseExact(timePart, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
        {
            return result;
        }

        return null;
    }
}