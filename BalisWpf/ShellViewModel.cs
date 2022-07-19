using System.Threading.Tasks;
using BalisStandard;
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
            Task.Delay(3000).Wait();
            StartBelStockPoller();
            StartTradingViewPollers();
            Task.Factory.StartNew(() => new InvestingPoller().Start(Model));

            StartKomBankPollers(iniFile, logFile);
        }

        private void StartKomBankPollers(IniFile iniFile, IMyLog logFile)
        {
            Task.Factory.StartNew( () => Model.KomBankListViewModel.Start(iniFile, logFile, _windowManager, _changesViewModel));
        }

        private void StartTradingViewPollers()
        {
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.EurUsd, Model.TradingViewVm.Rates.EurUsd, Model, 10));

            // не совпадает с Investing.com на который ориентируется наша биржа
            // Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.UsdRub, Model.TradingViewVm.Rates.UsdRub, Model, 500));

            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.UsdCny, Model.TradingViewVm.Rates.UsdCny, Model, 1000));


            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.UkOil, Model.TradingViewVm.Rates.UkOil, Model, 1500));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Gold, Model.TradingViewVm.Rates.Gold, Model, 2000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Spx, Model.TradingViewVm.Rates.SpSpx, Model, 2500));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Bnd, Model.TradingViewVm.Rates.AmexBnd, Model, 3000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Voo, Model.TradingViewVm.Rates.AmexVoo, Model, 3500));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Vix, Model.TradingViewVm.Rates.CboeVix, Model, 4000));


            // для прогноза эти курсы не нужны, справочно
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.EurRub, Model.TradingViewVm.Rates.EurRub, Model, 4500));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.CnyRub, Model.TradingViewVm.Rates.CnyRub, Model, 5000));
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