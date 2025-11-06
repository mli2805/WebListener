using System.Threading.Tasks;
using Caliburn.Micro;
using UtilsLib;

namespace BalisWpf
{
    public class ShellViewModel : PropertyChangedBase, IShell
    {
        private readonly IWindowManager _windowManager;
        private readonly ChangesViewModel _changesViewModel;
        public ShellVm Model { get; set; }

        public ShellViewModel(IniFile iniFile, IMyLog logFile, IWindowManager windowManager, ShellVm shellVm, ChangesViewModel changesViewModel)
        {
            _windowManager = windowManager;
            _changesViewModel = changesViewModel;
            Model = shellVm;

            StartNbRbPoller();
            Task.Delay(10_000).Wait();
            //StartBelStockPoller();

            var msPlaywrightPath = iniFile
                .Read(IniSection.Extractors, IniKey.MsPlaywrightPath,
                    @"c:\Users\Professional\AppData\Local\ms-playwright\chromium-1187\chrome-win\chrome.exe");
            Task.Factory.StartNew(() => new ForexPoller().Start(Model, logFile, msPlaywrightPath));

            StartKomBankPollers(iniFile, logFile);
        }

        private void StartKomBankPollers(IniFile iniFile, IMyLog logFile)
        {
            Task.Factory.StartNew( () => Model.KomBankListViewModel.Start(iniFile, logFile, _windowManager, _changesViewModel));
        }

        private void StartNbRbPoller()
        {
            Task.Factory.StartNew(() => new NbRbPoller().Start(Model));
        }

        private void StartBelStockPoller()
        {
            Task.Factory.StartNew(() => new BelStockPoller().Start(Model));
        }
    }
}