using System;
using System.Collections.Generic;

namespace BalisWpf
{
    public class TradingViewData
    {
        public TikerValues EurUsd{ get; set;} = new TikerValues();
        public TikerValues UsdRub{ get; set; } = new TikerValues();
        public TikerValues EurRub{ get; set; } = new TikerValues();
        public TikerValues UkOil{ get; set; } = new TikerValues();
        public TikerValues Gold{ get; set; } = new TikerValues();
        public TikerValues Spx { get; set; } = new TikerValues();
        public TikerValues Voo{ get; set; } = new TikerValues();
        public TikerValues Vix{ get; set; } = new TikerValues();
        public TikerValues Bnd{ get; set; } = new TikerValues();

        public List<string> F(DateTime lastCheck)
        {
            var result = new List<string>();
            result.Add("tradingview.com");
            result.Add($"{lastCheck}");
            result.Add("");
            result.Add($"Eur / Usd {EurUsd.Lp}  {EurUsd.Ch}");
            result.Add($"Usd / Rub {UsdRub.Lp}  {UsdRub.Ch}");
            result.Add($"Eur / Rub {EurRub.Lp}  {EurRub.Ch}");
            result.Add("");
            result.Add($"Brent ${UkOil.Lp}");
            result.Add($"Бочка {UkOil.Lp * UsdRub.Lp:0,0}");
            result.Add("");
            result.Add($"Gold ${Gold.Lp: 0,0.0}/ozt  {Gold.Ch}  {Gold.Chp}%");
            result.Add($" ${Gold.Lp / 31.1034768: 0.0}/g");
            result.Add("");
            result.AddRange(Fa(Spx, "S&P"));
            result.AddRange(Fa(Bnd, "BND"));
            result.AddRange(Fa(Voo, "VOO"));
            result.AddRange(Fa(Vix, "VIX"));
            
            return result;
        }

       
        private IEnumerable<string> Fa(TikerValues tv, string tikerName)
        {
            yield return ($"{tikerName} {tv.Lp}  {tv.Ch:+0.0;-0.0}  {tv.Chp:+0.0;-0.0}%");
            yield return ($"{tikerName} market: {tv.MarketStatus}");
            if (tv.MarketStatus == "pre-market")
                yield return ($"{tikerName} pre-market: {tv.Rtc} {tv.Rch:+0.0;-0.0}  {tv.Rchp:+0.0;-0.0}%");
        }
    }
}