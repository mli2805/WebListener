using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WebListener.Properties;

namespace WebListener
{
    public class Forex : INotifyPropertyChanged
    {
        private double _usdEur;
        private double _rubUsd;
        private double _eurRub;
        private double _brentUkOil;
        private DateTime _lastChecked;

        public DateTime LastChecked
        {
            get { return _lastChecked; }
            set
            {
                if (value.Equals(_lastChecked)) return;
                _lastChecked = value;
                OnPropertyChanged();
            }
        }

        public double UsdEur
        {
            get { return _usdEur; }
            set
            {
                if (value.Equals(_usdEur)) return;
                _usdEur = value;
                OnPropertyChanged();
            }
        }

        public double RubUsd
        {
            get { return _rubUsd; }
            set
            {
                if (value.Equals(_rubUsd)) return;
                _rubUsd = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BrentInRub));
            }
        }

        public double EurRub
        {
            get { return _eurRub; }
            set
            {
                if (value.Equals(_eurRub)) return;
                _eurRub = value;
                OnPropertyChanged();
            }
        }

        public double BrentUkOil
        {
            get { return _brentUkOil; }
            set
            {
                if (_brentUkOil.Equals(value)) return;
                _brentUkOil = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BrentInRub));
            }
        }

        public double BrentInRub => _brentUkOil * RubUsd;

        public bool IsRatherDifferent(Forex other)
        {
            if (other == null) return true;
            return (Math.Abs(_usdEur/other._usdEur - 1) > 0.005) || 
                    (Math.Abs(_rubUsd / other._rubUsd - 1) > 0.005) ||
                     (Math.Abs(_brentUkOil / other._brentUkOil - 1) > 0.005) ||
                      ((_lastChecked - other._lastChecked).TotalHours > 0);
        }

        public string ToFileString()
        {
            return $"{_lastChecked} usd/eur {_usdEur} rub/usd {_rubUsd} brent {_brentUkOil}";
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
