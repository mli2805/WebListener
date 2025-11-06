using System.Collections.Generic;
using System.Globalization;
using HtmlAgilityPack;

namespace BalisStandard;

public static class ProFinanceParser
{
    public static ForexRates Parse(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // собираем значения из трёх секций
        var values = new Dictionary<string, double>();

        // секция Курс рубля (id=kr)
        CollectFromSection(doc, "kr", values);

        // секция Товарные рынки (id=tr)
        CollectFromSection(doc, "tr", values);

        // секция Мировые валюты (id=vk)
        CollectFromSection(doc, "vk", values);
        
        return new ForexRates
        {
            EurUsd   = values.TryGet("EURUSD"),
            UsdCny   = values.TryGet("USDCNY"),
            BrentOil = values.TryGet("brent"),
            Gold     = values.TryGet("gold"),
            UsdRub   = values.TryGet("USDRUB"),
            EurRub   = values.TryGet("EURRUB"),
            CnyRub   = values.TryGet("CNYRUB")
        };
    }

    // helper
    static double? TryGet(this IDictionary<string, double> dict, string key)
    {
        return dict.TryGetValue(key, out var v) ? v : null;
    }

    private static void CollectFromSection(HtmlDocument doc, string sectionId, Dictionary<string, double> dest)
    {
        var section = doc.DocumentNode.SelectSingleNode($"//div[@id='{sectionId}']");
        if (section == null) return;

        // выбираем все строки с id (они содержат тикер в id)
        var rows = section.SelectNodes(".//div[contains(@class,'quote__row') and @id]");
        if (rows == null) return;

        foreach (var row in rows)
        {
            var id = row.GetAttributeValue("id", null);
            if (string.IsNullOrEmpty(id)) continue;

            double? value = GetRowValue(row);
            if (value.HasValue)
            {
                dest.Add(id, value.Value);
            }
        }
    }

    private static double? GetRowValue(HtmlNode row)
    {
        // пробуем bid и ask
        var bidNode = row.SelectSingleNode(".//div[contains(@class,'quote__row__cell--bid')]");
        var askNode = row.SelectSingleNode(".//div[contains(@class,'quote__row__cell--ask')]");

        double? bid = ParseDouble(bidNode?.InnerText);
        double? ask = ParseDouble(askNode?.InnerText);

        if (bid.HasValue && ask.HasValue) return (bid.Value + ask.Value) / 2.0;
        if (bid.HasValue) return bid.Value;
        if (ask.HasValue) return ask.Value;

        // fallback: некоторые секции используют .quote__row__cell--last или просто содержат единственное числовое значение
        var lastNode = row.SelectSingleNode(".//div[contains(@class,'quote__row__cell--last')]");
        var anyNumeric = ParseDouble(lastNode?.InnerText);
        if (anyNumeric.HasValue) return anyNumeric.Value;

        // ещё попытка: ищем первую ячейку, которая содержит число
        var cells = row.SelectNodes(".//div[contains(@class,'quote__row__cell')]");
        if (cells != null)
        {
            foreach (var c in cells)
            {
                var v = ParseDouble(c.InnerText);
                if (v.HasValue) return v;
            }
        }

        return null;
    }

    private static double? ParseDouble(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        s = s.Trim().Replace("+", "").Replace("%", "");
        // иногда значение содержит пробелы, запятые как разделитель тысяч — используем CultureInfo.InvariantCulture и заменим запятую на точку
        s = s.Replace("\u00A0", "").Replace(" ", "").Replace(",", ".");
        if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)) return d;
        return null;
    }
}