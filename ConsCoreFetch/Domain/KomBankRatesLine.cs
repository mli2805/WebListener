using System;

namespace ConsCoreFetch
{
    public class KomBankRatesLine
    {
        public int Id { get; set; }
        public string Bank{ get; set; }
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
        public DateTime LastCheck{ get; set; }
    }
}