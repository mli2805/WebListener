using System;
using BalisStandard;
using Caliburn.Micro;

namespace BalisWpf
{
    public class InvestingComViewModel : PropertyChangedBase
    {
        private DateTime _lastCheck;
        private double _usdRub;
        private double _eurRub;
        private double _eurUsd;
        private double _usdCny;
        private double _cnyRub;
        private double _brent;
        private double _gold;

        public DateTime LastCheck
        {
            get => _lastCheck;
            set
            {
                if (value == _lastCheck) return;
                _lastCheck = value;
                NotifyOfPropertyChange(() => LastCheck);
                NotifyOfPropertyChange(() => LastCheckStr);
            }
        }

        public double UsdRub
        {
            get => _usdRub;
            set
            {
                if (value == _usdRub) return;
                _usdRub = value;
                NotifyOfPropertyChange(() => UsdRub);
            }
        }

        public double EurRub
        {
            get => _eurRub;
            set
            {
                if (value == _eurRub) return;
                _eurRub = value;
                NotifyOfPropertyChange(() => EurRub);
            }
        }

        public double EurUsd
        {
            get => _eurUsd;
            set
            {
                if (value == _eurUsd) return;
                _eurUsd = value;
                NotifyOfPropertyChange(() => EurUsd);
            }
        }

        public double UsdCny
        {
            get => _usdCny;
            set
            {
                if (value.Equals(_usdCny)) return;
                _usdCny = value;
                NotifyOfPropertyChange(() => UsdCny);
            }
        }

        public double CnyRub
        {
            get => _cnyRub;
            set
            {
                if (value == _cnyRub) return;
                _cnyRub = value;
                NotifyOfPropertyChange(() => CnyRub);
            }
        }

        public double Brent
        {
            get => _brent;
            set
            {
                if (value.Equals(_brent)) return;
                _brent = value;
                NotifyOfPropertyChange(() => Brent);
                NotifyOfPropertyChange(() => BrentStr);
                NotifyOfPropertyChange(() => BrentRubStr);
            }
        }

        public string BrentStr => $"{Brent}";
        public string BrentRubStr => $"{Brent * UsdRub:F0}";

        public double Gold
        {
            get => _gold;
            set
            {
                if (value.Equals(_gold)) return;
                _gold = value;
                NotifyOfPropertyChange(() => Gold);
                NotifyOfPropertyChange(() => GoldStr);
                NotifyOfPropertyChange(() => GoldStr2);
            }
        }

        public string GoldStr => $"${Gold:0,0.00} / ozt.";
        public string GoldStr2 => $"${Gold / 31.1034768:0.00} / g.";

        public string LastCheckStr => $"Investing.com {LastCheck:h:mm:ss}";


        public RatesForForecast Forex => new RatesForForecast() { UsdRub = UsdRub, EurUsd = EurUsd, UsdCny = UsdCny };

    }
}
