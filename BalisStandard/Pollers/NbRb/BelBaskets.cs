using System;
using System.Collections.Generic;
using System.Linq;

namespace BalisStandard
{
    public static class BelBaskets
    {
        public static readonly List<BasketWeights> Weights = new List<BasketWeights>()
        {
            new BasketWeights() {DateFrom = new DateTime(2009, 1, 1), Usd = 1.0 / 3, Euro = 1.0 / 3, Rur = 1.0 / 3},
            new BasketWeights() {DateFrom = new DateTime(2016, 1, 1), Usd = 0.3, Euro = 0.3, Rur = 0.4}, // дата примерно
            new BasketWeights() {DateFrom = new DateTime(2016, 11, 1), Usd = 0.3, Euro = 0.2, Rur = 0.5},
            new BasketWeights() {DateFrom = new DateTime(2022, 7, 15), Usd = 0.3, Euro = 0.1, Rur = 0.5, Cny = 0.1},
            new BasketWeights() {DateFrom = new DateTime(2022, 12, 12), Usd = 0.3, Euro = 0, Rur = 0.6, Cny = 0.1},
        };

        public static double Calculate(double usd, double eur, double rub, double cny)
        {
            return Calculate(usd, eur, rub, cny, DateTime.Today);
        }

        public static double Calculate(double usd, double eur, double rub, double cny, DateTime date)
        {
            var weights = Weights.Last(b => b.DateFrom.Date <= date.Date);

            return Math.Pow(usd, weights.Usd) *
                         Math.Pow(eur, weights.Euro) *
                         Math.Pow(rub / 100, weights.Rur) *
                         Math.Pow(cny / 10, weights.Cny);
        }
    }
}