using System.Collections.ObjectModel;
using Caliburn.Micro;
using Extractors;

namespace WebListenerRx
{
    public class OneBankViewModel : PropertyChangedBase
    {
        private string _bankTitle;
        public string BankTitle
        {
            get { return _bankTitle; }
            set
            {
                if (value == _bankTitle) return;
                _bankTitle = value;
                NotifyOfPropertyChange();
            }
        }

        private ObservableCollection<KomBankRates> _rows = new ObservableCollection<KomBankRates>();
        public ObservableCollection<KomBankRates> Rows
        {
            get { return _rows; }
            set
            {
                if (Equals(value, _rows)) return;
                _rows = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
