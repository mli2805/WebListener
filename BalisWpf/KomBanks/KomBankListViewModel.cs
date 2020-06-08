using System.Collections.ObjectModel;
using System.Windows;
using BalisStandard;
using Caliburn.Micro;

namespace BalisWpf
{
    public class KomBankListViewModel : PropertyChangedBase
    {
        public ObservableCollection<KomBankViewModel> Banks { get; set; } = new ObservableCollection<KomBankViewModel>();

        public async void Start()
        {
            {
                var bibViewModel = await new KomBankViewModel(KomBankE.Bib).GetLastFive();
                Application.Current.Dispatcher.Invoke(() => Banks.Add(bibViewModel));
                var priorViewModel = await new KomBankViewModel(KomBankE.Prior).GetLastFive();
                Application.Current.Dispatcher.Invoke(() => Banks.Add(priorViewModel));
                var mmbViewModel = await new KomBankViewModel(KomBankE.Mmb).GetLastFive();
                Application.Current.Dispatcher.Invoke(() => Banks.Add(mmbViewModel));
                var bgpbViewModel = await new KomBankViewModel(KomBankE.Bgpb).GetLastFive();
                Application.Current.Dispatcher.Invoke(() => Banks.Add(bgpbViewModel));
            }
        }
    }
}