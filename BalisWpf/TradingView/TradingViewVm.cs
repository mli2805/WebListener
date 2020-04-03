using System;
using System.Collections.Generic;
using System.Globalization;

namespace BalisWpf
{
    public class TradingViewVm
    {
        public TikerValues EurUsd { get; set; } = new TikerValues();
        public TikerValues UsdRub { get; set; } = new TikerValues();
        public TikerValues EurRub { get; set; } = new TikerValues();
        public TikerValues UkOil { get; set; } = new TikerValues();
        public TikerValues Gold { get; set; } = new TikerValues();
        public TikerValues SpSpx { get; set; } = new TikerValues();
        public TikerValues AmexVoo { get; set; } = new TikerValues();
        public TikerValues CboeVix { get; set; } = new TikerValues();
        public TikerValues AmexBnd { get; set; } = new TikerValues();

        public List<string> F(DateTime lastCheck)
        {
            var result = new List<string>();
            result.Add("tradingview.com");
            result.Add($"{lastCheck}");
            result.Add("");
            result.Add($"Eur / Usd {EurUsd.Lp}  {EurUsd.Ch:+0.000;-0.000}");
            result.Add($"{TimeStr(EurUsd)}");
            result.Add($"Usd / Rub {UsdRub.Lp}  {UsdRub.Ch:+0.00;-0.00}");
            result.Add($"Eur / Rub {EurRub.Lp}  {EurRub.Ch:+0.00;-0.00}");
            result.Add("");
            result.Add($"Brent ${UkOil.Lp}  ({UkOil.Ch:+0.00;-0.00} {UkOil.Chp:+0.00;-0.00}%)");
            result.Add($"Бочка {UkOil.Lp * UsdRub.Lp:0,0}");
            result.Add($"{TimeStr(UkOil)}");
            result.Add("");
            result.Add($"Gold ${Gold.Lp: 0,0.0}/ozt  ({Gold.Ch:+0.00;-0.00}  {Gold.Chp:+0.0;-0.0}%)");
            result.Add($" ${Gold.Lp / 31.1034768: 0.0}/g");
            result.Add("");
            result.AddRange(FSp(SpSpx, "SP:S&P_500"));
            result.AddRange(FAmex(AmexBnd, "AMEX:BND"));
            result.AddRange(FAmex(AmexVoo, "AMEX:VOO"));
            result.AddRange(FCboe(CboeVix, "CBOE:VIX"));

            return result;
        }



        private IEnumerable<string> FCboe(TikerValues tv, string tikerName)
        {
            if (tv.CurrentSession == "out_of_session")
            {
                yield return $"{tikerName} {tv.Lp}  (вчера {tv.Ch:+0.00;-0.00}  {tv.Chp:+0.00;-0.00}%)";
                yield return $" рынок закрыт";
            }
            else if (tv.CurrentSession == "pre_market")
            {
                var ch = tv.PrevClosePrice - tv.OpenPrice;
                var chp = ch / tv.OpenPrice * 100;
                yield return $"{tikerName} {tv.PrevClosePrice}  (вчера {ch:+0.00;-0.00}  {chp:+0.00;-0.00}%)";
                yield return $" премаркет: {tv.Lp} {tv.Ch:+0.00;-0.00}  {tv.Chp:+0.00;-0.00}%";
            }
            else // tv.CurrentSession == "market"
            {
                yield return $"{tikerName} {tv.Lp}  ({tv.Ch:+0.00;-0.00}  {tv.Chp:+0.00;-0.00}%)";
                yield return $"{TimeStr(tv)}";
            }
        }

        private IEnumerable<string> FSp(TikerValues tv, string tikerName)
        {
            if (tv.CurrentSession == "out_of_session")
            {
                yield return $"{tikerName} {tv.Lp}  (вчера {tv.Ch:+0.00;-0.00}  {tv.Chp:+0.00;-0.00}%)";
                yield return $" рынок закрыт";
            }
            else // tv.CurrentSession == "market"
            {
                yield return $"{tikerName} {tv.Lp}  ({tv.Ch:+0.00;-0.00}  {tv.Chp:+0.00;-0.00}%)";
                yield return $"{TimeStr(tv)}";

            }
        }

        private IEnumerable<string> FAmex(TikerValues tv, string tikerName)
        {
            if (tv.CurrentSession == "out_of_session")
            {
                yield return $"{tikerName} {tv.Lp}  (вчера {tv.Ch:+0.00;-0.00}  {tv.Chp:+0.00;-0.00}%)";
                yield return $" рынок закрыт";
            }
            else if (tv.CurrentSession == "pre_market" && tv.MarketStatus == "pre-market")
            {
                yield return $"{tikerName} {tv.Lp}  (вчера {tv.Ch:+0.00;-0.00}  {tv.Chp:+0.00;-0.00}%)";
                yield return $" премаркет: {tv.Rtc} {tv.Rch:+0.00;-0.00}  {tv.Rchp:+0.00;-0.00}%";
            }
            else // tv.CurrentSession == "market"
            {
                yield return $"{tikerName} {tv.Lp}  ({tv.Ch:+0.00;-0.00}  {tv.Chp:+0.00;-0.00}%)";
                yield return $"{TimeStr(tv)}";
            }
        }

        private string TimeStr(TikerValues tv)
        {
            if (tv.OpenTime.Year == 1) return "";
            var word = tv.CurrentSession == "market" ? "открыт с" : "откроется в";
            var ny = tv.OpenTime.AddHours(-4);
            var minsk = tv.OpenTime.AddHours(3);
            var date = tv.OpenTime.ToString("dd/MM", CultureInfo.GetCultureInfo("en-US"));
            return $" {word} {minsk:HH:mm} (NY {ny:HH:mm}) {date} tz:{tv.TimeZone}";
        }
    }
}