using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace HeavyKeyProcessing
{
    public class SomeControlViewModel : PropertyChangedBase
    {
        private string _testText = "Initial value";
        public string TestText
        {
            get { return _testText; }
            set
            {
                _testText = value; 
             //   HeavyCalculation(_testText);
                Task.Factory.StartNew(() => HeavyCalculationAnotherThread(_testText));
                HeavyCalculationAnotherThread(_testText);
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

        private void HeavyCalculation(string param)
        {
            Thread.Sleep(300);
            ResultText = "result-" + param;
        }

        private void HeavyCalculationAnotherThread(string param)
        {
            Thread.Sleep(300);
            Application.Current.Dispatcher.Invoke(() => ResultText = "result-" + param); // sync, GUI thread
        }

    }
}
