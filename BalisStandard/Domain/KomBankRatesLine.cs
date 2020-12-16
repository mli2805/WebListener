using System;

namespace BalisStandard
{
    public class KomBankRatesLine
    {
        public int Id { get; set; }
        public string Bank{ get; set; }
        public DateTime LastCheck{ get; set; }
        public DateTime StartedFrom{ get; set; }
        public double UsdA{ get; set; }
        public double UsdB{ get; set; }
        public double EurA{ get; set; }
        public double EurB{ get; set; }
        public double RubA{ get; set; }
        public double RubB{ get; set; }
        public double EurUsdA{ get; set; }
        public double EurUsdB{ get; set; }
        public double RubUsdA{ get; set; }
        public double RubUsdB{ get; set; }
        public double RubEurA{ get; set; }
        public double RubEurB{ get; set; }

        public bool IsDifferent(KomBankRatesLine anotherLine)
        {
            return anotherLine == null || 
                   !UsdA.Equals(anotherLine.UsdA) || !UsdB.Equals(anotherLine.UsdB) ||
                   !EurA.Equals(anotherLine.EurA) || !EurB.Equals(anotherLine.EurB) ||
                   !RubA.Equals(anotherLine.RubA) || !RubB.Equals(anotherLine.RubB) ||
                   !EurUsdA.Equals(anotherLine.EurUsdA) || !EurUsdB.Equals(anotherLine.EurUsdB) ||
                   !RubUsdA.Equals(anotherLine.RubUsdA) || !RubUsdB.Equals(anotherLine.RubUsdB) ||
                   !RubEurA.Equals(anotherLine.RubEurA) || !RubEurB.Equals(anotherLine.RubEurB);
        }

        public override string ToString()
        {
            return $"{Bank} from {StartedFrom}: USD {UsdA}-{UsdB};  EUR {EurA}-{EurB};  RUB {RubA}-{RubB} ....";
        }
    }
}