﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using BalisStandard;
using Caliburn.Micro;
using UtilsLib;

namespace BalisWpf
{
    public class KomBankListViewModel : PropertyChangedBase
    {
        private IMyLog _logFile;
        public ObservableCollection<KomBankViewModel> Banks { get; set; } = new ObservableCollection<KomBankViewModel>();
        private List<KomBankE> _firstPageList = new List<KomBankE>()
            { KomBankE.Bib, KomBankE.Bnb, KomBankE.Alfa, KomBankE.Bveb, KomBankE.Bgpb, KomBankE.Mmb, KomBankE.Prior, KomBankE.Mtb, KomBankE.Vtb };

        public async void Start(IniFile iniFile, IMyLog logFile, IWindowManager windowManager, ChangesViewModel changesViewModel)
        {
            _logFile = logFile;
            _logFile.AppendLine("Kom banks listening started");

            foreach (var komBank in _firstPageList)
            {
                var viewModel = await new KomBankViewModel(iniFile, komBank, _logFile, windowManager, changesViewModel).GetSomeLast();
                Application.Current.Dispatcher.Invoke(() => Banks.Add(viewModel));
                var unused = await Task.Factory.StartNew(viewModel.StartPolling);
            }
        }
    }
}