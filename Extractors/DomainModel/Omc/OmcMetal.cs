using System;
using System.Globalization;

namespace Extractors
{
    public class OmcMetal
    {
        public double BankBuyByn { get; set; }
        public double BankBuyUsd { get; set; }
        public double BankSellByn { get; set; }
        public double BankSellUsd { get; set; }

        public OmcMetal()
        {
        }

        public OmcMetal(string ss0, string ss1, string ss2, string ss3)
        {
            try
            {
                BankBuyByn  = double.Parse(ss0,new CultureInfo("en-US"));
                BankBuyUsd  = double.Parse(ss1,new CultureInfo("en-US"));
                BankSellByn = double.Parse(ss2,new CultureInfo("en-US"));
                BankSellUsd = double.Parse(ss3,new CultureInfo("en-US"));
            }
            catch (Exception)
            {
                BankBuyByn  = -1;
                BankBuyUsd  = -1;
                BankSellByn = -1;
                BankSellUsd = -1;
            }
        }

    public bool Equals(OmcMetal omcMetal)
        {
            return BankBuyByn.Equals(omcMetal.BankBuyByn) && BankBuyUsd.Equals(omcMetal.BankBuyUsd) &&
                     BankSellByn.Equals(omcMetal.BankSellByn) && BankSellUsd.Equals(omcMetal.BankSellUsd);
        }

        public string ToFileString()
        {
            return $"{BankBuyByn} {BankBuyUsd} {BankSellByn} {BankSellUsd}";
        }
    }
}