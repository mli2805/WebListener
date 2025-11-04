using System.Collections.Generic;

// from 21 02 2025
namespace BalisStandard.AlfaBank
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Cash
    {
        public string title { get; set; }
        public string icon { get; set; }
        public Purchase purchase { get; set; }
        public Sell sell { get; set; }
    }

    public class CurrenciesDatum
    {
        public string text { get; set; }
        public string date { get; set; }
        public Value value { get; set; }
    }

    public class InitialItem
    {
        public List<CurrenciesDatum> currenciesData { get; set; }
    }

    public class Purchase
    {
        public string value { get; set; }
        public object change { get; set; }
    }

    public class Root
    {
        public List<InitialItem> initialItems { get; set; }
        public string title { get; set; }
    }

    public class Sell
    {
        public string value { get; set; }
        public object change { get; set; }
    }

    public class Value
    {
        public List<Cash> cash { get; set; }
    }


}
