using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using BalisStandard;
using BalisWpf.Annotations;

namespace BalisWpf
{
    public class NbRbVm : INotifyPropertyChanged
    {
        private NbRates _previousTradeDay = new NbRates();
        private NbRates _today = new NbRates();
        private NbRates _tomorrow = new NbRates();

        public NbRates PreviousTradeDay
        {
            get => _previousTradeDay;
            set
            {
                if (Equals(value, _previousTradeDay)) return;
                _previousTradeDay = value;
                OnPropertyChanged("PreviousTradeDayToScreen");
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
                OnPropertyChanged("TodayToScreen");
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
        public List<string> PreviousTradeDayToScreen => DayToScreen(PreviousTradeDay);
        public List<string> TodayToScreen => DayToScreen(Today);

        public List<string> F()
        {
            var result = new List<string>();
            result.AddRange(FDay(PreviousTradeDay));
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
            result.Add("");
            result.Add($"Корзина  {day.Basket:0.0000}");
            result.Add("");
            return result;
        }

        public List<string> NamesToScreen =>
            new List<string>()
            {
                "",
                "Usd",
                "Euro",
                "100 Rub",
                "",
                "Корзина",
                "",
                "Eur / Usd",
                "Usd / Rub",
                "Eur / Rub",
            };


        public List<string> DayToScreen(NbRates day)
        {
            var result = new List<string>();
            result.Add($"на {day.Date.ToString("dd/MM", CultureInfo.GetCultureInfo("en-US"))}");
            result.Add($"{day.Usd:0.0000}");
            result.Add($"{day.Eur:0.0000}");
            result.Add($"{day.Rub:0.0000}");
            result.Add("");
            result.Add($"{day.Basket:0.0000}");
            result.Add("");
            result.Add($"{day.EurUsd:0.0000}");
            result.Add($"{day.UsdRub:0.00}");
            result.Add($"{day.EurRub:0.00}");
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