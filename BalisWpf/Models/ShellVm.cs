using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BalisWpf.Annotations;

namespace BalisWpf
{
    public class ShellVm : INotifyPropertyChanged
    {
        public TradingViewData TradingViewData { get; set; } = new TradingViewData();

        private DateTime _lastCheck;
        public DateTime LastCheck
        {
            get => _lastCheck;
            set
            {
                if (value.Equals(_lastCheck)) return;
                _lastCheck = value;
                OnPropertyChanged();
                OnPropertyChanged("TradingViewList");
            }
        }

        public List<string> TradingViewList => TradingViewData.F(_lastCheck);

    

        public string Test { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}