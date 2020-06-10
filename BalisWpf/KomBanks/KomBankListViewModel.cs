using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using BalisStandard;
using Caliburn.Micro;
using UtilsLib;

namespace BalisWpf
{
    public class KomBankListViewModel : PropertyChangedBase
    {
        private IMyLog _logFile;
        private BalisSignalRClient _balisSignalRClient;
        public ObservableCollection<KomBankViewModel> Banks { get; set; } = new ObservableCollection<KomBankViewModel>();
        private List<KomBankE> _firstPageList = new List<KomBankE>(){ KomBankE.Bib, KomBankE.Prior, KomBankE.Mmb, KomBankE.Bgpb};

        public async void Start(IMyLog logFile)
        {
            _logFile = logFile;
            _balisSignalRClient = new BalisSignalRClient(_logFile);

            foreach (var komBank in _firstPageList)
            {
                var viewModel = await new KomBankViewModel(komBank, _logFile, _balisSignalRClient).GetSomeLast();
                Application.Current.Dispatcher.Invoke(() => Banks.Add(viewModel));

            }

//            var bibViewModel = await new KomBankViewModel(KomBankE.Bib, _balisSignalRClient).GetLastFive();
//            Application.Current.Dispatcher.Invoke(() => Banks.Add(bibViewModel));
//            var priorViewModel = await new KomBankViewModel(KomBankE.Prior, _balisSignalRClient).GetLastFive();
//            Application.Current.Dispatcher.Invoke(() => Banks.Add(priorViewModel));
//            var mmbViewModel = await new KomBankViewModel(KomBankE.Mmb, _balisSignalRClient).GetLastFive();
//            Application.Current.Dispatcher.Invoke(() => Banks.Add(mmbViewModel));
//            var bgpbViewModel = await new KomBankViewModel(KomBankE.Bgpb, _balisSignalRClient).GetLastFive();
//            Application.Current.Dispatcher.Invoke(() => Banks.Add(bgpbViewModel));

            _balisSignalRClient.Start();
        }
    }
}