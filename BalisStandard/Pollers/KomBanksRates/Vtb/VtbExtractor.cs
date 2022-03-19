using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BalisStandard
{
    public class VtbExtractor : IRatesLineExtractor
    {
        public string BankTitle => KomBankE.Vtb.ToString().ToUpper();
        private const string Url = "https://www.vtb.by/sites/default/files/rates.xml";

        public async Task<KomBankRatesLine> GetRatesLineAsync()
        {
            var xmlRates = await ((HttpWebRequest)WebRequest.Create(Url))
                .InitializeForKombanks()
                .GetDataAsync();
            if (string.IsNullOrEmpty(xmlRates))
                return null;

            try
            {
                return Parse(xmlRates);
            }
            catch (Exception e)
            {
                File.WriteAllText($@"c:\tmp\{BankTitle}_{DateTime.Now:yyyy.MM.dd-HH.mm.ss}.txt", $@"{e.Message} in {BankTitle} parser");
                return null;
            }
        }

        private KomBankRatesLine Parse(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            var root = doc.Root;
            if (root == null)
                return null;

            var dateStr = root.Attribute("date")?.Value;

            var cardRatesElement = doc.Descendants("card").First();
            var cardRates = cardRatesElement.Elements().Select(ParseVtbCardRate).ToList();

            var conversionRatesElement = doc.Descendants("conversion").First();
            var conversionRates = conversionRatesElement.Elements().Select(ParseVtbConversionRate).ToList();

            var result = new KomBankRatesLine()
            {
                Bank = BankTitle,
                StartedFrom = DateTime.ParseExact(dateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture),
                LastCheck = DateTime.Now,
                UsdA = cardRates.First(r => r.Currency == Currency.Usd).Buy,
                UsdB = cardRates.First(r => r.Currency == Currency.Usd).Sell,
                EurA = cardRates.First(r => r.Currency == Currency.Eur).Buy,
                EurB = cardRates.First(r => r.Currency == Currency.Eur).Sell,
                RubA = cardRates.First(r => r.Currency == Currency.Rub).Buy,
                RubB = cardRates.First(r => r.Currency == Currency.Rub).Sell,

                EurUsdA = conversionRates.First(r => r.CurrencyFrom == Currency.Eur && r.CurrencyTo == Currency.Usd).Value,
                EurUsdB = conversionRates.First(r => r.CurrencyFrom == Currency.Usd && r.CurrencyTo == Currency.Eur).Value,
                RubUsdA = conversionRates.First(r => r.CurrencyFrom == Currency.Usd && r.CurrencyTo == Currency.Rub).Value,
                RubUsdB = conversionRates.First(r => r.CurrencyFrom == Currency.Rub && r.CurrencyTo == Currency.Usd).Value,
                RubEurA = conversionRates.First(r => r.CurrencyFrom == Currency.Eur && r.CurrencyTo == Currency.Rub).Value,
                RubEurB = conversionRates.First(r => r.CurrencyFrom == Currency.Rub && r.CurrencyTo == Currency.Eur).Value,
            };
            return result;
        }

        private static VtbCardRate ParseVtbCardRate(XElement cardRate)
        {
            var rate = new VtbCardRate();
            var code = cardRate.Element("code")?.Value;
            if (code != null)
                rate.Currency = (Currency)Enum.Parse(typeof(Currency), code, true);
            var buy = cardRate.Element("buy")?.Value;
            if (buy != null)
                rate.Buy = double.Parse(buy, CultureInfo.GetCultureInfo("en-US"));
            var sell = cardRate.Element("sell")?.Value;
            if (sell != null)
                rate.Sell = double.Parse(sell, CultureInfo.GetCultureInfo("en-US"));
            return rate;
        }

        private VtbConversionRate ParseVtbConversionRate(XElement conversionRate)
        {
            var rate = new VtbConversionRate();
            var codeFrom = conversionRate.Element("codeFrom")?.Value;
            if (codeFrom != null)
                rate.CurrencyFrom = (Currency)Enum.Parse(typeof(Currency), codeFrom, true);
            var codeTo = conversionRate.Element("codeTo")?.Value;
            if (codeTo != null)
                rate.CurrencyTo = (Currency)Enum.Parse(typeof(Currency), codeTo, true);
            var value = conversionRate.Element("value")?.Value;
            if (value != null)
                rate.Value = double.Parse(value, CultureInfo.GetCultureInfo("en-US"));
            return rate;
        }
    }


}
