using System;

namespace BanksListener
{
    public class BelStockArchiveDay
    {
        public BelStockArchiveOneCurrencyDay Usd { get; set; }
        public BelStockArchiveOneCurrencyDay Eur { get; set; }
        public BelStockArchiveOneCurrencyDay Rub { get; set; }
    }

    public class BelStockArchiveOneCurrencyDay
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Currency Currency { get; set; }
        public double First { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double Last { get; set; }
        public double TurnoverInByn { get; set; }
        public double TurnoverInCurrency { get; set; }
        public int Count { get; set; }
    }

}