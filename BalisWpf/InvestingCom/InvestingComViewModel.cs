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

        public string LastCheckStr => $"Investing.com {LastCheck:h:mm:ss}";
      

        public RatesForForecast Forex => new RatesForForecast() { UsdRub = UsdRub, EurUsd = EurUsd, UsdCny = UsdCny };

    }
}
