using System;

namespace BalisWpf
{
    public class BelStockArchiveDay
    {
        public DateTime Date { get; set; }
        public double UsdRate { get; set; }
        public double UsdTurnover { get; set; }
        public double EuroRate { get; set; }
        public double EuroTurnover { get; set; }
        public double RubRate { get; set; }
        public double RubTurnover { get; set; }

        public double TotalTurnover => UsdTurnover + EuroTurnover + RubTurnover;
    }
}