using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BalisStandard;
using BalisWpf.Annotations;

namespace BalisWpf
{
    public class ShellVm : INotifyPropertyChanged
    {
        public NbRbVm NbRbVm { get; set; } = new NbRbVm();
        public BelStockVm BelStockVm { get; set; } = new BelStockVm();
        public TradingViewVm TradingViewVm { get; set; } = new TradingViewVm();

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

        public List<string> TradingViewList => TradingViewVm.F(_lastCheck);

    

        public string Test { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}