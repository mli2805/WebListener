using System.Threading.Tasks;
using BalisStandard;
using Caliburn.Micro;
using UtilsLib;

namespace BalisWpf
{
    public class KomBankTnCViewModel : Screen
    {
        private readonly KomBankE _komBankE;
        private readonly IniFile _iniFile;
        private readonly IMyLog _logFile;
        private readonly IWindowManager _windowManager;
        public KomBankViewModel KomBankViewModel { get; set; }

        public KomBankTnCViewModel(KomBankE komBankE, IniFile iniFile, IMyLog logFile, IWindowManager windowManager)
        {
            _komBankE = komBankE;
            _iniFile = iniFile;
            _logFile = logFile;
            _windowManager = windowManager;
        }

        protected override void OnViewLoaded(object view)
        {
            DisplayName = _komBankE.GetAbbreviation();
        }

        public async Task Initialize()
        {
            KomBankViewModel = await new KomBankViewModel(_iniFile, _komBankE, _logFile, _windowManager).GetSomeLast();

        }

      
    }
}
