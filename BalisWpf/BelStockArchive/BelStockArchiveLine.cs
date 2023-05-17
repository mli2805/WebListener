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

        public string EuroTurnoverStr => GetFormatted(EuroTurnover);

        public double RubRate { get; set; }
        public double RubTurnover { get; set; }

        public double CnyRate { get; set; }
        public double CnyTurnover { get; set; }

        public double TotalTurnover => UsdTurnover + EuroTurnover + RubTurnover + CnyTurnover;

        public double Basket => Date == DateTime.MinValue ? 0 : BelBaskets.Calculate(UsdRate, EuroRate, RubRate, CnyRate, Date);


        private string GetFormatted(double number)
        {
           return number < 0.0001
                ? ""
                : number < 0.001
                    ? string.Format($"{number:0.0000}")
                    : number < 0.01
                        ? string.Format($"{number:0.000}")
                        : number < 0.1
                            ? string.Format($"{number:0.00}")
                            : string.Format($"{number:0.0}");
        } 
      
    }
}