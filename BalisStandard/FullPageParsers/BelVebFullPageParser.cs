using System;
using System.Collections.Generic;
using System.Globalization;
using HtmlAgilityPack;

namespace BalisStandard;

public class BelVebFullPageParser : IFullPageParser
{
    public KomBankRatesLine? ParseKomBankRatesFromHtml(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var result = new KomBankRatesLine
        {
            Bank = KomBankE.Bveb.ToString().ToUpper(),
            LastCheck = DateTime.Now,
            StartedFrom = ExtractStartedFrom(doc),
        };

        var currencyMap = new Dictionary<string, Action<double, double>>
        {
            ["USD"] = (a, b) => { result.UsdA = a; result.UsdB = b; },
            ["EUR"] = (a, b) => { result.EurA = a; result.EurB = b; },
            ["RUB"] = (a, b) => { result.RubA = a; result.RubB = b; },
            ["CNY"] = (a, b) => { result.CnyA = a; result.CnyB = b; },
            ["EUR/USD"] = (a, b) => { result.EurUsdA = a; result.EurUsdB = b; },
            ["USD/RUB"] = (a, b) => { result.RubUsdA = a; result.RubUsdB = b; },
            ["EUR/RUB"] = (a, b) => { result.RubEurA = a; result.RubEurB = b; },
            ["USD/CNY"] = (a, b) => { result.UsdCnyA = a; result.UsdCnyB = b; },
            ["EUR/CNY"] = (a, b) => { result.EurCnyA = a; result.EurCnyB = b; },
            ["CNY/RUB"] = (a, b) => { result.CnyRubA = a; result.CnyRubB = b; },
        };

        var cells = doc.DocumentNode.SelectNodes("//div[contains(@class,'currency-item__cell')]");
        if (cells != null)
        {
            foreach (var cell in cells)
            {
                var captionNode = cell.SelectSingleNode(".//div[contains(@class,'currency-item__currency-caption')]/span");
                var buyNode = cell.SelectSingleNode(".//div[contains(@class,'currency-item__course')][1]//div[contains(@class,'currency-item__course-item')]");
                var sellNode = cell.SelectSingleNode(".//div[contains(@class,'currency-item__course')][2]//div[contains(@class,'currency-item__course-item')]");

                if (captionNode == null || buyNode == null || sellNode == null)
                    continue;

                var caption = captionNode.InnerText.Trim();
                var buyText = buyNode.InnerText.Trim();
                var sellText = sellNode.InnerText.Trim();

                if (string.IsNullOrEmpty(caption) || !double.TryParse(buyText, NumberStyles.Any, CultureInfo.InvariantCulture, out var buy) ||
                    !double.TryParse(sellText, NumberStyles.Any, CultureInfo.InvariantCulture, out var sell))
                    continue;

                string key = NormalizeCaption(caption);
                if (currencyMap.TryGetValue(key, out var setter))
                {
                    setter(buy, sell);
                }
            }
        }

        return result;
    }

    private static DateTime ExtractStartedFrom(HtmlDocument doc)
    {
        var dateInput = doc.DocumentNode.SelectSingleNode("//input[@data-selected-date]");
        if (dateInput != null && DateTime.TryParse(dateInput.GetAttributeValue("data-selected-date", ""), out var date))
        {
            return date;
        }
        return DateTime.MinValue;
    }

    private static string NormalizeCaption(string raw)
    {
        raw = raw.Replace("*", "").Replace("1 ", "").Replace("100 ", "").Replace("10 ", "").Trim();
        return raw.Split(' ')[0].ToUpper();
    }
}