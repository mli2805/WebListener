using System;
using System.Globalization;

namespace WebListener
{
    public class BelStockArchiveOneCurrencyDay
    {
        public Currency Currency { get; set; }

        public double First { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double Last { get; set; }
        public double TurnoverInByn { get; set; }
        public double TurnoverInCurrency { get; set; }
        public int Count { get; set; }

        public double Average => EvaluateAverage();

        private double EvaluateAverage()
        {
            var k = Currency == Currency.Rub ? 100 : 1;
            return Math.Round(TurnoverInByn*k/TurnoverInCurrency, 4);
        }

        public BelStockArchiveOneCurrencyDay()
        {
        }

        public BelStockArchiveOneCurrencyDay(string str)
        {
            var parts = str.Split();
            Currency = (Currency) Enum.Parse(typeof (Currency), parts[0]);
            First = double.Parse(parts[1], NumberStyles.Any, new CultureInfo("en-US"));
            Min = double.Parse(parts[2], NumberStyles.Any, new CultureInfo("en-US"));
            Max = double.Parse(parts[3], NumberStyles.Any, new CultureInfo("en-US"));
            Last = double.Parse(parts[4], NumberStyles.Any, new CultureInfo("en-US"));
            TurnoverInByn = double.Parse(parts[5], NumberStyles.Any, new CultureInfo("en-US"));
            TurnoverInCurrency = double.Parse(parts[7], NumberStyles.Any, new CultureInfo("en-US"));
            Count = int.Parse(parts[9]);
        }
        public string ToFileString()
        {
            return string.Format(new CultureInfo("en-US"), "{0:0.0000} {1:0.0000} {2:0.0000} {3:0.0000} {4:0.0000} bln {5:0.0000} mln {6} ", First, Min, Max,
                Last, TurnoverInByn, TurnoverInCurrency, Count);
        }
    }
}
