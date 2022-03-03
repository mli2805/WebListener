using System;
using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace BalisStandard
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Purchase    {
        public double value { get; set; } 
        public object change { get; set; } 
    }

    public class Sell    {
        public double value { get; set; } 
        public object change { get; set; } 
    }

    public class ExchangeRate    {
        public string title { get; set; } 
        public string icon { get; set; } 
        public Purchase purchase { get; set; } 
        public Sell sell { get; set; } 
    }

    public class ConversionRate    {
        public string title { get; set; } 
        public Purchase purchase { get; set; } 
        public Sell sell { get; set; } 
    }

    public class Value    {
        public List<ExchangeRate> exchangeRate { get; set; } 
        public List<ConversionRate> conversionRate { get; set; } 
    }

    public class CurrenciesData    {
        public string text { get; set; } 
        public DateTime date { get; set; } 
        public Value value { get; set; } 
    }

    public class InitialItem    {
        public string title { get; set; } 
        public string address { get; set; } 
        public string titleCaption { get; set; } 
        public string conversionActivatorText { get; set; } 
        public List<CurrenciesData> currenciesData { get; set; } 
    }

    public class AlfaRoot    {
        public object filterData { get; set; } 
        public List<InitialItem> initialItems { get; set; } 
    }


}
