using System.ComponentModel;
using System.Runtime.CompilerServices;
using WebListener.Properties;

namespace WebListener
{
    public class BelStockCurrency : INotifyPropertyChanged
    {
        private double _average;
        private string _volume;
        private double _lastDeal;

        public double Average
        {
            get { return _average; }
            set
            {
                if (value.Equals(_average)) return;
                _average = value;
                OnPropertyChanged();
            }
        }

        public string Volume
        {
            get { return _volume; }
            set
            {
                if (value.Equals(_volume)) return;
                _volume = value;
                OnPropertyChanged();
            }
        }

        public double LastDeal
        {
            get { return _lastDeal; }
            set
            {
                if (value.Equals(_lastDeal)) return;
                _lastDeal = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}