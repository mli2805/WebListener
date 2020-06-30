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
        private List<KomBankE> _firstPageList = new List<KomBankE>(){ KomBankE.Bib, KomBankE.Prior, KomBankE.Mmb, KomBankE.Bveb, KomBankE.Bgpb};

        public async void Start(IMyLog logFile)
        {
            _logFile = logFile;
            _logFile.AppendLine("Kom banks listening started");
            _balisSignalRClient = new BalisSignalRClient(_logFile);

            foreach (var komBank in _firstPageList)
            {
                var viewModel = await new KomBankViewModel(komBank, _logFile, _balisSignalRClient).GetSomeLast();
                Application.Current.Dispatcher.Invoke(() => Banks.Add(viewModel));

            }
            _balisSignalRClient.Start();
        }
    }
}