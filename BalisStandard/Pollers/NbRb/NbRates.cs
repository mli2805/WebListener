using System;

namespace BalisStandard
{
    public class NbRates
    {
        public DateTime Date { get; set; }
        public double Usd { get; set; }
        public double Eur { get; set; }
        public double Rub { get; set; }
        public double Basket => NbBasket.Calculate(Usd, Eur, Rub / 100);
        public double EurUsd => Eur / Usd;
        public double UsdRub => Usd * 100 / Rub;
        public double EurRub => Eur * 100 / Rub;

    }
}
