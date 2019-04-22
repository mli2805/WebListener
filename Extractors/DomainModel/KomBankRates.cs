using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Extractors.Properties;

namespace Extractors
{
    public class KomBankRatesDto
    {
        public KomBank2 Bank;
        public DateTime StartedFrom;
        public double UsdA;
        public double UsdB;
        public double EurA;
        public double EurB;
        public double RubA;
        public double RubB;
        public double EurUsdA;
        public double EurUsdB;
        public double RubUsdA;
        public double RubUsdB;
        public double RubEurA;
        public double RubEurB;
        public DateTime LastCheck;
    }

    public class KomBankRates : INotifyPropertyChanged
    {
        #region class properties
        public string Bank { get; set; }
        public string StartedFrom { get; set; }
        public string StartedFromForGrid => FormatStartedFromForGrid(StartedFrom);
        public double UsdA { private get; set; }
        public double UsdB { private get; set; }
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
        public string UsdByrRub => _lastCheck >= new DateTime(2016, 7, 1) ?
            PairToString3(UsdA * 100 / RubB, UsdB * 100 / RubA) : 
            PairToString3(UsdA / RubB, UsdB / RubA);

        public double RubEurA { private get; set; }
        public double RubEurB { private get; set; }
        public string RubEur => PairToString3(RubEurA, RubEurB);
        public string EurByrRub => _lastCheck >= new DateTime(2016, 7, 1) ? 
            PairToString3(EurA * 100 / RubB, EurB * 100/ RubA) :
            PairToString3(EurA / RubB, EurB / RubA);

        private DateTime _lastCheck;
        public DateTime LastCheck
        {
            get { return _lastCheck; }
            set
            {
                if (value.Equals(_lastCheck)) return;
                _lastCheck = value;
                OnPropertyChanged(nameof(LastCheckForGrid));
            }
        }
        public string LastCheckForGrid => $"{_lastCheck:dd.MM.yyy\nHH:mm:ss}";

        #endregion

        private static string FormatStartedFromForGrid(string startedFrom)
        {
            var pos = startedFrom.IndexOf(":", StringComparison.Ordinal);
            if (pos == -1) return startedFrom;
            return startedFrom.Substring(0, pos - 2) + "\n  " + startedFrom.Substring(pos - 2);
        }
        private string CurrencyPairToString(double a, double b)
        {
            var fl = Bank == "БГПБ" && LastCheck > new DateTime(2019,4,21) ? ">10К" : "";
            return (a.Equals(0) || b.Equals(0)) ? "" :
                $"{a:#,0.####} - {b:#,0.####}  {fl}\n ( {b - a:#,0.####}  {(b - a)*100/b:#,0.##}% ) ";
        }
        private static string CrossPairToString(double a, double b)
        {
            return (a.Equals(0) || b.Equals(0)) ? "" :
                $"{a:#,0.####} - {b:#,0.####}\n ( {(b - a)*100/b:#,0.##}% ) ";
        }
        private string PairToString3(double a, double b)
        {
            return (a.Equals(0) || b.Equals(0)) ? "" :
                $"{a:#,0.###} - {b:#,0.###}\n ( {(b - a)*100/b:#,0.##}% ) ";
        }
        public string ToFileString()
        {
            return string.Format(new CultureInfo("en-US"), "{0} {1} USD {2} - {3} EUR {4} - {5} RUR {6} - {7} " +
                "EUR/USD {8} - {9} RUR/USD {10} - {11} RUR/EUR {12} - {13} {14}",
                Bank, StartedFrom.Replace(' ', '_'), UsdA, UsdB, EurA, EurB, RubA, RubB, 
                EurUsdA, EurUsdB, RubUsdA, RubUsdB, RubEurA, RubEurB, LastCheck.ToString("dd-MM-yyyy hh:mm:ss"));
        }

        public string ToDisplayEcopress()
        {
            return string.Format(new CultureInfo("en-US"), "На ecopress.by инсайд для {0} от {1}\n \n " +
                    " USD {2} - {3} EUR {4} - {5} RUR {6} - {7} ",
                Bank, LastCheck.ToString("dd-MM-yyyy hh:mm:ss"), UsdA, UsdB, EurA, EurB, RubA, RubB);
        }


        public bool IsDifferent(KomBankRates anotherLine)
        {
            return anotherLine == null || 
                 !UsdA.Equals(anotherLine.UsdA) || !UsdB.Equals(anotherLine.UsdB) ||
                 !EurA.Equals(anotherLine.EurA) || !EurB.Equals(anotherLine.EurB) ||
                 !RubA.Equals(anotherLine.RubA) || !RubB.Equals(anotherLine.RubB) ||
                 !EurUsdA.Equals(anotherLine.EurUsdA) || !EurUsdB.Equals(anotherLine.EurUsdB) ||
                 !RubUsdA.Equals(anotherLine.RubUsdA) || !RubUsdB.Equals(anotherLine.RubUsdB) ||
                 !RubEurA.Equals(anotherLine.RubEurA) || !RubEurB.Equals(anotherLine.RubEurB);
        }

        public KomBankRates()
        {
        }

        public KomBankRates(string str)
        {
            if (str == null) return;
            var ss = str.Split();
            try
            {
                Bank = ss[0];
                StartedFrom = ss[1].Replace('_', ' ');
                UsdA    = double.Parse(ss[ 3],new CultureInfo("en-US"));
                UsdB    = double.Parse(ss[ 5],new CultureInfo("en-US"));
                EurA    = double.Parse(ss[ 7],new CultureInfo("en-US"));
                EurB    = double.Parse(ss[ 9],new CultureInfo("en-US"));
                RubA    = double.Parse(ss[11],new CultureInfo("en-US"));
                RubB    = double.Parse(ss[13],new CultureInfo("en-US"));
                EurUsdA = double.Parse(ss[15],new CultureInfo("en-US"));
                EurUsdB = double.Parse(ss[17],new CultureInfo("en-US"));
                RubUsdA = double.Parse(ss[19],new CultureInfo("en-US"));
                RubUsdB = double.Parse(ss[21],new CultureInfo("en-US"));
                RubEurA = double.Parse(ss[23],new CultureInfo("en-US"));
                RubEurB = double.Parse(ss[25],new CultureInfo("en-US"));
                LastCheck = ss.Length == 29 
                    ? DateTime.Parse(ss[26] + " " + ss[27] + " " + ss[28], new CultureInfo("en-US")) 
                    : DateTime.Parse(ss[26] + " " + ss[27], new CultureInfo("ru-RU"));
            }
            catch (Exception)
            {
                Console.WriteLine(@"Exception during KomBankRate parsing from string");
            }
        }

        public KomBankRates Clone()
        {
            return (KomBankRates) MemberwiseClone();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
