using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Extractors.Properties;

namespace Extractors
{
    public class NbRates : INotifyPropertyChanged
    {
        private DateTime _date;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (value == _date) return;
                _date = value;
                OnPropertyChanged();
            }
        }

        public double Usd { get; set; }
        public double Eur { get; set; }
        public double Rub { get; set; }
        public double Basket => NbBasket.Calculate(Usd, Eur, Rub / 100);
        public double EurUsd => Eur/Usd;
        public double UsdRub => Usd*100/Rub;
        public double EurRub => Eur*100/Rub;

        public bool Equals(NbRates other)
        {
            if (other == null) return false;
            return Usd.Equals(other.Usd) && Eur.Equals(other.Eur) && Rub.Equals(other.Rub);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
