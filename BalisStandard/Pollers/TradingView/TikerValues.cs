using System;

namespace BalisStandard
{
    public class TikerValues
    {
        public double Lp { get; set; }
        public double Ch { get; set; }
        public double Chp { get; set; }
        public string MarketStatus { get; set; }
        public string CurrentSession { get; set; }
        public double Rtc { get; set; } // pre-market value
        public double Rch { get; set; } 
        public double Rchp { get; set; }
        public double PrevClosePrice { get; set; }
        public double OpenPrice { get; set; } // previous ?
        public DateTime OpenTime { get; set; }
        public string TimeZone { get; set; }
    }
}