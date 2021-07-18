using System.Collections.Generic;

namespace BalisStandard
{
    public class BelVebRate
    {
        public double value { get; set; }
        public int dinamic { get; set; }
    }

 
    public class BelVebCurrency
    {
        public string id { get; set; }
        public string currency_cod { get; set; }
        public string currency_qty { get; set; }
        public string cuttency_img { get; set; }
        public BelVebRate buy_rate { get; set; }
        public BelVebRate sale_rate { get; set; }
    }

    public class Root1
    {
        public List<BelVebCurrency> rates { get; set; }
        public string timetext { get; set; }
    }

    //===============================================

    public class SetOfRates
    {
        public double USD { get; set; }
        public double RUB { get; set; }
        public double BYN { get; set; }
        public double EUR { get; set; }
    }

   

    public class EUR
    {
        public SetOfRates purchase { get; set; }
        public SetOfRates sell { get; set; }
        public string symbol { get; set; }
    }

    public class USD
    {
        public SetOfRates purchase { get; set; }
        public SetOfRates sell { get; set; }
        public string symbol { get; set; }
    }

    public class RUB
    {
        public SetOfRates purchase { get; set; }
        public SetOfRates sell { get; set; }
        public string symbol { get; set; }
    }

    public class BYN
    {
        public SetOfRates purchase { get; set; }
        public SetOfRates sell { get; set; }
        public string symbol { get; set; }
    }

    public class ConvertionRates
    {
        public EUR EUR { get; set; }
        public USD USD { get; set; }
        public RUB RUB { get; set; }
        public BYN BYN { get; set; }
    }

    public class Root2
    {
        public ConvertionRates rates { get; set; }
    }

}
