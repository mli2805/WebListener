using System.ComponentModel;
using System.Runtime.CompilerServices;
using Extractors.Properties;

namespace Extractors
{
    public class Calculations : INotifyPropertyChanged
    {
        private double _usdRate;
        private double _annualRate;
        private int _days;

        public double AnnualRate
        {
            get { return _annualRate; }
            set
            {
                if (value.Equals(_annualRate)) return;
                _annualRate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProfitProcent));
                OnPropertyChanged(nameof(ProfitByr));
            }
        }

        public int Days
        {
            get { return _days; }
            set
            {
                if (value == _days) return;
                _days = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProfitProcent));
                OnPropertyChanged(nameof(ProfitByr));
            }
        }

        public double ProfitProcent { get { return AnnualRate * Days / 365; } }
        public double ProfitByr { get { return _usdRate * AnnualRate / 100 * Days / 365; } }

        public Calculations(double annualRate, int days, double usdRate)
        {
            AnnualRate = annualRate;
            Days = days;
            _usdRate = usdRate;
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
