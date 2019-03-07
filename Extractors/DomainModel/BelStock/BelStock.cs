using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Extractors.Properties;

namespace Extractors
{
    public class BelStock : INotifyPropertyChanged
    {
        private BelStockState _tradingState;
        private DateTime _tradingDate;
        private DateTime _lastChecked;
        private BelStockCurrency _usd;
        private BelStockCurrency _eur;
        private BelStockCurrency _rub;

        public BelStockState TradingState
        {
            get { return _tradingState; }
            set
            {
                if (value == _tradingState) return;
                _tradingState = value;
                OnPropertyChanged();
            }
        }
        public DateTime TradingDate
        {
            get { return _tradingDate; }
            set
            {
                if (value.Equals(_tradingDate)) return;
                _tradingDate = value;
                OnPropertyChanged();
            }
        }
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

        public BelStockCurrency Usd
        {
            get { return _usd; }
            set
            {
                if (Equals(value, _usd)) return;
                _usd = value;
                OnPropertyChanged();
            }
        }

        public BelStockCurrency Eur
        {
            get { return _eur; }
            set
            {
                if (Equals(value, _eur)) return;
                _eur = value;
                OnPropertyChanged();
            }
        }

        public BelStockCurrency Rub
        {
            get { return _rub; }
            set
            {
                if (Equals(value, _rub)) return;
                _rub = value;
                OnPropertyChanged();
            }
        }

        public BelStock()
        {
            Usd = new BelStockCurrency();
            Eur = new BelStockCurrency();
            Rub = new BelStockCurrency();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
