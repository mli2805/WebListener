using System.ComponentModel;
using System.Runtime.CompilerServices;

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
}