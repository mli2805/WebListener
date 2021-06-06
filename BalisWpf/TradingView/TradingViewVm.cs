using System;
using System.Collections.Generic;
using BalisStandard;
using Caliburn.Micro;

namespace BalisWpf
{
    public class TradingViewVm : Screen
    {
        public TradingViewRates Rates { get; set; } = new TradingViewRates();

        private DateTime _lastCheck;
        public DateTime LastCheck
        {
            get => _lastCheck;
            set
            {
                if (value.Equals(_lastCheck)) return;
                _lastCheck = value;
                NotifyOfPropertyChange("Title");
                NotifyOfPropertyChange("LeftPanel");
                NotifyOfPropertyChange("RightPanel");
            }
        }

        public string Title => $"Tradingview.com {_lastCheck:dd.MM HH:mm:ss}";
        public List<string> LeftPanel =>
            new List<string>()
            {
                "",
                $"Eur / Usd {Rates.EurUsd.Lp:0.0000}  {Rates.EurUsd.Ch:+0.000;-0.000}",
                $"Usd / Rub {Rates.UsdRub.Lp:0.000}  {Rates.UsdRub.Ch:+0.00;-0.00}",
                "",
                $"Eur / Rub {Rates.EurRub.Lp:0.000}  {Rates.EurRub.Ch:+0.00;-0.00}",
                "",
                $"Brent ${Rates.UkOil.Lp}",
                $"     ({Rates.UkOil.Ch:+0.00;-0.00} {Rates.UkOil.Chp:+0.00;-0.00}%)",
                $"����� {Rates.UkOil.Lp * Rates.UsdRub.Lp:0,0}",
            };

        public List<string> RightPanel => BuildRightPanel();
        private List<string> BuildRightPanel()
        {
            var result = new List<string>
            {
                "",
                $"Gold ${Rates.Gold.Lp: 0,0.0}/ozt  ({Rates.Gold.Ch:+0.00;-0.00}  {Rates.Gold.Chp:+0.0;-0.0}%)",
                $" ${Rates.Gold.Lp / 31.1034768: 0.0}/g",
                ""
            };
            result.AddRange(FSp(Rates.SpSpx, "SP:S&P_500"));
            result.AddRange(FAmex(Rates.AmexBnd, "AMEX:BND"));
            result.AddRange(FAmex(Rates.AmexVoo, "AMEX:VOO"));
            result.AddRange(FCboe(Rates.CboeVix, "CBOE:VIX"));
            return result;
        }
        
        private IEnumerable<string> FCboe(TikerValues tv, string tikerName)
        {
            if (tv.CurrentSession == "out_of_session")
            {
                yield return $"{tikerName} {tv.Lp}  (��� {tv.Ch:+0.00;-0.00}  {tv.Chp:+0.00;-0.00}%)";
//                yield return $" ����� ������";
            }
            else if (tv.CurrentSession == "pre_market")
            {
                var ch = tv.PrevClosePrice - tv.OpenPrice;
                var chp = ch / tv.OpenPrice * 100;
                yield return $"{tikerName} {tv.PrevClosePrice}  (��� {ch:+0.00;-0.00}  {chp:+0.00;-0.00}%)";
//                yield return $" ���������: {tv.Lp} {tv.Ch:+0.00;-0.00}  {tv.Chp:+0.00;-0.00}%";
            }
            else // tv.CurrentSession == "market"
            {
                yield return $"{tikerName} {tv.Lp}  ({tv.Ch:+0.00;-0.00}  {tv.Chp:+0.00;-0.00}%)";
//                yield return $"{TimeStr(tv)}";
            }
        }

        private IEnumerable<string> FSp(TikerValues tv, string tikerName)
        {
            if (tv.CurrentSession == "out_of_session")
            {
                yield return $"{tikerName} {tv.Lp}  (��� {tv.Ch:+0.00;-0.00}  {tv.Chp:+0.00;-0.00}%)";
//                yield return $" ����� ������";
            }
            else // tv.CurrentSession == "market"
            {
                yield return $"{tikerName} {tv.Lp}  ({tv.Ch:+0.00;-0.00}  {tv.Chp:+0.00;-0.00}%)";
//                yield return $"{TimeStr(tv)}";
            }
        }

        private IEnumerable<string> FAmex(TikerValues tv, string tikerName)
        {
            if (tv.CurrentSession == "out_of_session")
            {
                yield return $"{tikerName} {tv.Lp}  (��� {tv.Ch:+0.00;-0.00}  {tv.Chp:+0.00;-0.00}%)";
//                yield return $" ����� ������";
            }
            else if (tv.CurrentSession == "pre_market" && tv.MarketStatus == "pre-market")
            {
                yield return $"{tikerName} {tv.Lp}  (��� {tv.Ch:+0.00;-0.00}  {tv.Chp:+0.00;-0.00}%)";
//                yield return $" ���������: {tv.Rtc} {tv.Rch:+0.00;-0.00}  {tv.Rchp:+0.00;-0.00}%";
            }
            else // tv.CurrentSession == "market"
            {
                yield return $"{tikerName} {tv.Lp}  ({tv.Ch:+0.00;-0.00}  {tv.Chp:+0.00;-0.00}%)";
//                yield return $"{TimeStr(tv)}";
            }
        }

//        private string TimeStr(TikerValues tv)
//        {
//            if (tv.OpenTime.Year == 1) return "";
//            var word = tv.CurrentSession == "market" ? "������ �" : "��������� �";
//            var ny = tv.OpenTime.AddHours(-4);
//            var minsk = tv.OpenTime.AddHours(3);
//            var date = tv.OpenTime.ToString("dd/MM", CultureInfo.GetCultureInfo("en-US"));
//            return $" {word} {minsk:HH:mm} (NY {ny:HH:mm}) {date} tz:{tv.TimeZone}";
//        }
    }
}