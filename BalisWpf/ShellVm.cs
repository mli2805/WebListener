using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BalisWpf.Annotations;

namespace BalisWpf
{
    public class ShellVm : INotifyPropertyChanged
    {
        public NbRbVm NbRbVm { get; set; } = new NbRbVm();
        public BelStockVm BelStockVm { get; set; } = new BelStockVm();
        public TradingViewVm TradingViewVm { get; set; } = new TradingViewVm();
        public ForecastVm ForecastVm { get; set;  } = new ForecastVm();

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
                if (ForecastVm.CurrentNbRates == null)
                {
                    if (NbRbVm.Today.Date.Year > 1)
                        ForecastVm.Initialize(NbRbVm.Today);
                }
                else ForecastVm.CalculateNewRates(TradingViewVm.Rates);
            }
        }

        public List<string> TradingViewList => TradingViewVm.F(_lastCheck);

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}