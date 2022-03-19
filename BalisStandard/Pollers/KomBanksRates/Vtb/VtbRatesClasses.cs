namespace BalisStandard
{
    public class VtbCardRate
    {
        public Currency Currency { get; set; }
        public double Buy { get; set; }
        public double Sell { get; set; }
    }

    public class VtbConversionRate
    {
        public Currency CurrencyFrom { get; set; }
        public Currency CurrencyTo { get; set; }
        public double Value { get; set; }
    }
}
