using System;
using System.Globalization;
using Caliburn.Micro;

namespace Balis2021
{
    public class KomBankRateVm : PropertyChangedBase
    {
        #region class properties
        public int Id { get; set; }
        public string Bank { get; set; }
        public DateTime StartedFrom { get; set; }
        public string StartedFromForGrid => FormatStartedFromForGrid(StartedFrom);
        public double UsdA { get; set; }
        public bool UsdAIsBest { get; set; }
        public double UsdB { get; set; }
        public string Usd => CurrencyPairToString(UsdA, UsdB);
        public double EurA { private get; set; }
        public double EurB { private get; set; }
        public string Eur => CurrencyPairToString(EurA, EurB);
        public double RubA { private get; set; }
        public double RubB { private get; set; }
        public string Rub => CurrencyPairToString(RubA, RubB);
        public double EurUsdA { private get; set; }
        public double EurUsdB { private get; set; }
        public string EurUsd => CrossPairToString(EurUsdA, EurUsdB);
        public string EurByrUsd => CrossPairToString(EurA / UsdB, EurB / UsdA);

        public double RubUsdA { private get; set; }
        public double RubUsdB { private get; set; }
        public string RubUsd => PairToString3(RubUsdA, RubUsdB);
        public string UsdByrRub => CrossThroughByn3(UsdA, UsdB, RubA, RubB);

       
        public double RubEurA { private get; set; }
        public double RubEurB { private get; set; }
        public string RubEur => PairToString3(RubEurA, RubEurB);
        public string EurByrRub => CrossThroughByn3(EurA, EurB, RubA, RubB);

        private DateTime _lastCheck;

        public DateTime LastCheck
        {
            get { return _lastCheck; }
            set
            {
                if (value.Equals(_lastCheck)) return;
                _lastCheck = value;
                NotifyOfPropertyChange(nameof(LastCheckForGrid));
            }
        }

        private string _state = "";
        public string State
        {
            get => _state;
            set
            {
                if (value == _state) return;
                _state = value;
                NotifyOfPropertyChange();
            }
        }

        public string LastCheckForGrid => $"{_lastCheck:dd.MM.yyy\nHH:mm:ss}";

        public void SetIfExpired()
        {
            var timeSpan = TimeSpan.FromSeconds((Bank == "BNB" ? 60 : 15) * 3);
            State = DateTime.Now - LastCheck > timeSpan ? "Expired" : "";
        }
        #endregion

        private static string FormatStartedFromForGrid(DateTime timestamp)
        {
            var startedFrom = timestamp.ToString(CultureInfo.CurrentUICulture);
            var pos = startedFrom.IndexOf(":", StringComparison.Ordinal);
            if (pos == -1) return startedFrom;
            return startedFrom.Substring(0, pos - 2) + "\n  " + startedFrom.Substring(pos - 2);
        }

        private string CurrencyPairToString(double a, double b)
        {
            var fl = Bank == "БГПБ" && LastCheck > new DateTime(2019, 4, 21) ? ">10К" : "";
            return (a.Equals(0) || b.Equals(0)) ? "" :
                $"{a:#,0.####} - {b:#,0.####}  {fl}\n ( {b - a:#,0.####}  {(b - a) * 100 / b:#,0.##}% ) ";
        }

        private static string CrossPairToString(double a, double b)
        {
            return (a.Equals(0) || b.Equals(0)) ? "" :
                $"{a:#,0.####} - {b:#,0.####}\n ( {(b - a) * 100 / b:#,0.##}% ) ";
        }

        private string PairToString3(double a, double b)
        {
            return (a.Equals(0) || b.Equals(0)) ? "" :
                $"{a:#,0.###} - {b:#,0.###}\n ( {(b - a) * 100 / b:#,0.##}% ) ";
        }

        private string CrossThroughByn3(double curA, double curB, double rubA, double rubB)
        {
            if (rubA.Equals(0) || rubB.Equals(0)) return "";
            return _lastCheck >= new DateTime(2016, 7, 1) ?
                PairToString3(curA * 100 / rubB, curB * 100 / rubA) :
                PairToString3(curA / rubB, curB / rubA);
        }

    }
}
