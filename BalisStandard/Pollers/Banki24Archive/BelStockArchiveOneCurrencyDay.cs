using System;

namespace BalisStandard
{
    public class BelStockArchiveOneCurrency
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Currency Currency { get; set; }
        public double Average { get; set; }
        public double First { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double Last { get; set; }
        public double TurnoverInCurrency { get; set; }
        public double TurnoverInByn { get; set; }
        public double TurnoverInUsd { get; set; }
        public int Count { get; set; }
    }
}