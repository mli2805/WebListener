using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Extractors.Properties;

namespace Extractors
{
    public class AmexVoo : INotifyPropertyChanged
    {
        private double _lp;
        private double _pre;
        private double _ask;
        private double _bid;

        public double Lp
        {
            get => _lp;
            set
            {
                if (value.Equals(_lp)) return;
                _lp = value;
                OnPropertyChanged();
            }
        }

        public double Pre
        {
            get => _pre;
            set
            {
                if (value.Equals(_pre)) return;
                _pre = value;
                OnPropertyChanged();
            }
        }

        public double Ask
        {
            get => _ask;
            set
            {
                if (value.Equals(_ask)) return;
                _ask = value;
                OnPropertyChanged();
            }
        }

        public double Bid
        {
            get => _bid;
            set
            {
                if (value.Equals(_bid)) return;
                _bid = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Forex : INotifyPropertyChanged
    {
        private double _usdEur;
        private double _rubUsd;
        private double _eurRub;
        private double _brentUkOil;
        private DateTime _lastChecked;
        private AmexVoo _voo = new AmexVoo();

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

        public AmexVoo Voo
        {
            get => _voo;
            set  {
                if (_voo.Equals(value)) return;
                _voo = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(_voo));
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
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
