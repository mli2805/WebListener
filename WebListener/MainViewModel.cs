using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WebListener.DomainModel;
using WebListener.DomainModel.BelStock;
using WebListener.DomainModel.Omc;
using WebListener.Properties;

namespace WebListener
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<KomBankRates> RowsBib { get; set; }
        public ObservableCollection<KomBankRates> RowsPrior { get; set; }
        public ObservableCollection<KomBankRates> RowsMoMi { get; set; }
        public ObservableCollection<KomBankRates> RowsBelGaz { get; set; }
        public ObservableCollection<KomBankRates> RowsBps { get; set; }

        private NbRates _nbRates1;
        public NbRates NbRates1
        {
            get { return _nbRates1; }
            set
            {
                if (Equals(value, _nbRates1)) return;
                _nbRates1 = value;
                OnPropertyChanged();
            }
        }

        private NbRates _nbRates0;
        public NbRates NbRates0
        {
            get { return _nbRates0; }
            set
            {
                if (Equals(value, _nbRates0)) return;
                _nbRates0 = value;
                OnPropertyChanged();
            }
        }

        public Forex Forex
        {
            get { return _forex; }
            set
            {
                if (Equals(value, _forex)) return;
                _forex = value;
                OnPropertyChanged();
            }
        }

        private ForecastRates _forecast;
        public ForecastRates Forecast
        {
            get { return _forecast; }
            set
            {
                if (Equals(value, _forecast)) return;
                _forecast = value;
                OnPropertyChanged();
            }
        }

        private BelStockWrapped _belStockWrapped;
        public BelStockWrapped BelStockWrapped
        {
            get { return _belStockWrapped; }
            set
            {
                if (Equals(value, _belStockWrapped)) return;
                _belStockWrapped = value;
                OnPropertyChanged();
            }
        }

        private Calculations _calculations;
        public Calculations Calculations
        {
            get { return _calculations; }
            set
            {
                if (Equals(value, _calculations)) return;
                _calculations = value;
                OnPropertyChanged();
            }
        }

        private OmcWrapped _omcWrapped;
        private Forex _forex;
        private string _ecopressLastCheck;

        public OmcWrapped OmcWrapped
        {
            get { return _omcWrapped; }
            set
            {
                if (Equals(value, _omcWrapped)) return;
                _omcWrapped = value;
                OnPropertyChanged();
            }
        }

        public string EcopressLastCheck
        {
            get { return _ecopressLastCheck; }
            set
            {
                if (Equals(value, _ecopressLastCheck)) return;
                _ecopressLastCheck = value;
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
