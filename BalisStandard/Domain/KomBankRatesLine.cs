using System;

namespace BalisStandard
{
    public class KomBankRatesLine
    {
        public int Id { get; set; }
        public string Bank { get; set; }
        public DateTime LastCheck { get; set; }
        public DateTime StartedFrom { get; set; }

        public double UsdA { get; set; }
        public double UsdB { get; set; }
        public double EurA { get; set; }
        public double EurB { get; set; }
        public double RubA { get; set; }
        public double RubB { get; set; }
        public double CnyA { get; set; }
        public double CnyB { get; set; }

        public double EurUsdA { get; set; }
        public double EurUsdB { get; set; }

        public double RubUsdA { get; set; }
        public double RubUsdB { get; set; }

        public double RubEurA { get; set; }
        public double RubEurB { get; set; }

        public double UsdCnyA { get; set; }
        public double UsdCnyB { get; set; }

        public double EurCnyA { get; set; } // новый курс EUR/CNY
        public double EurCnyB { get; set; }

        public double CnyRubA { get; set; } // новый курс CNY/RUB
        public double CnyRubB { get; set; }

        public bool IsDifferent(KomBankRatesLine anotherLine)
        {
            return anotherLine == null ||
                   !UsdA.Equals(anotherLine.UsdA) || !UsdB.Equals(anotherLine.UsdB) ||
                   !EurA.Equals(anotherLine.EurA) || !EurB.Equals(anotherLine.EurB) ||
                   !RubA.Equals(anotherLine.RubA) || !RubB.Equals(anotherLine.RubB) ||
                   !CnyA.Equals(anotherLine.CnyA) || !CnyB.Equals(anotherLine.CnyB) ||
                   !EurUsdA.Equals(anotherLine.EurUsdA) || !EurUsdB.Equals(anotherLine.EurUsdB) ||
                   !RubUsdA.Equals(anotherLine.RubUsdA) || !RubUsdB.Equals(anotherLine.RubUsdB) ||
                   !RubEurA.Equals(anotherLine.RubEurA) || !RubEurB.Equals(anotherLine.RubEurB) ||
                   !UsdCnyA.Equals(anotherLine.UsdCnyA) || !UsdCnyB.Equals(anotherLine.UsdCnyB) ||
                   !EurCnyA.Equals(anotherLine.EurCnyA) || !EurCnyB.Equals(anotherLine.EurCnyB) ||
                   !CnyRubA.Equals(anotherLine.CnyRubA) || !CnyRubB.Equals(anotherLine.CnyRubB);
        }

        public override string ToString()
        {
            return $"{Bank} from {StartedFrom}  (last check {LastCheck}) " + Environment.NewLine +
                   $"USD {UsdA}-{UsdB};  EUR {EurA}-{EurB};  RUB {RubA}-{RubB}; CNY {CnyA}-{CnyB}; " + Environment.NewLine +
                   $"EUR/USD {EurUsdA}-{EurUsdB}; USD/RUB {RubUsdA}-{RubUsdB}; EUR/RUB {RubEurA}-{RubEurB}; " +
                   $"USD/CNY {UsdCnyA}-{UsdCnyB}; EUR/CNY {EurCnyA}-{EurCnyB}; CNY/RUB {CnyRubA}-{CnyRubB}";
        }
    }
}
