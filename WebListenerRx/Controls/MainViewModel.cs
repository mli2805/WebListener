using System.Threading;
using Caliburn.Micro;

namespace WebListenerRx
{
    public class MainViewModel : PropertyChangedBase
    {
        private bool _isEnabled;
        public MainVm MainVm { get; set; } = new MainVm();

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value == _isEnabled) return;
                _isEnabled = value;
                NotifyOfPropertyChange();
            }
        }

        private string _testText = "Initial value";
        public string TestText
        {
            get { return _testText; }
            set
            {
                _testText = value; 
                HeavyCalculation(_testText);
            }
        }

        private string _resultText;
        public string ResultText
        {
            get { return _resultText; }
            set
            {
                if (value == _resultText) return;
                _resultText = value;
                NotifyOfPropertyChange();
            }
        }

        public MainViewModel()
        {
            MainVm.PropertyChanged += MainVm_PropertyChanged;
        }

        private void MainVm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AllBanksLoaded")
                StartKomBanksPolling();
        }

        public void StartKomBanksPolling()
        {
            IsEnabled = true;
        }

        private void HeavyCalculation(string param)
        {
            Thread.Sleep(300);
            ResultText = "result-" + param;
        }

        public void LoadHistory()
        {
            HistoryLoader.LoadKombanksHistory(MainVm);
        }


    }
}
