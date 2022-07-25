using System;
using BalisStandard;

namespace BalisWpf
{
    public class BelStockArchiveLine
    {
        public DateTime Date { get; set; }
        public string Timestamp { get; set; }
        public double UsdRate { get; set; }
        public double UsdTurnover { get; set; }
        public double EuroRate { get; set; }
        public double EuroTurnover { get; set; }
        public double RubRate { get; set; }
        public double RubTurnover { get; set; }

        public double CnyRate { get; set; }
        public double CnyTurnover { get; set; }

        public double TotalTurnover => UsdTurnover + EuroTurnover + RubTurnover + CnyTurnover;

        public double Basket => NbBasket.Calculate(UsdRate, EuroRate, RubRate / 100, CnyRate / 10);

    }
}