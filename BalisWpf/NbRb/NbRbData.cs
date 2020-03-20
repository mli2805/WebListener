using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using BalisStandard;
using BalisWpf.Annotations;

namespace BalisWpf
{
    public class NbRbData: INotifyPropertyChanged
    {
        private NbRates _yesterday = new NbRates();
        private NbRates _today = new NbRates();
        private NbRates _tomorrow = new NbRates();

        public NbRates Yesterday
        {
            get => _yesterday;
            set
            {
                if (Equals(value, _yesterday)) return;
                _yesterday = value;
                OnPropertyChanged("NbRbList");
            }
        }

        public NbRates Today
        {
            get => _today;
            set
            {
                if (Equals(value, _today)) return;
                _today = value;
                OnPropertyChanged("NbRbList");
            }
        }

        public NbRates Tomorrow
        {
            get => _tomorrow;
            set
            {
                if (Equals(value, _tomorrow)) return;
                _tomorrow = value;
                OnPropertyChanged("NbRbList");
            }
        }

        public List<string> NbRbList => F();

        public List<string> F()
        {
            var result = new List<string>();
            result.AddRange(FDay(Yesterday));
            result.AddRange(FDay(Today));
            if (Tomorrow.Date.Year > 1)
                result.AddRange(FDay(Tomorrow));
            return result;
        }

        public List<string> FDay(NbRates day)
        {
            var result = new List<string>();
            result.Add($"{day.Date.ToString("dd/MM", CultureInfo.GetCultureInfo("en-US"))}");
            result.Add($"Usd  {day.Usd}");
            result.Add($"Eur  {day.Eur}");
            result.Add($"Rub  {day.Rub}");
            result.Add($"Корзина  {day.Basket:0.0000}");
            result.Add("");
            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}