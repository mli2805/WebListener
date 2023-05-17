using System;

namespace BalisStandard
{
    public class NbRates
    {
        public DateTime Date { get; set; }
        public double Usd { get; set; }
        public double Eur { get; set; }
        public double Rub { get; set; }
        public double Cny { get; set; }

        // public double Basket => ForecasterFromBasket.Calculate(Usd, Eur, Rub / 100, Cny / 10);
        public double Basket => BelBaskets.Calculate(Usd, Eur, Rub, Cny);
        public double EurUsd => Eur / Usd;
        public double UsdRub => Usd * 100 / Rub;
        public double EurRub => Eur * 100 / Rub;
        public double UsdCny => Usd * 10 / Cny;

        public bool Equals(NbRates other)
        {
            if (other == null) return false;
            return Usd.Equals(other.Usd) && Eur.Equals(other.Eur) && Rub.Equals(other.Rub) && Cny.Equals(other.Cny);
        }

    }
}
