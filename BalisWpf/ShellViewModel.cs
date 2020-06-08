using System.Threading.Tasks;
using BalisStandard;
using UtilsLib;

namespace BalisWpf
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        public ShellVm Model { get; set; }

        public ShellViewModel(IMyLog logFile)
        {
            Model = new ShellVm();

            StartNbRbPoller();
            Task.Delay(3000).Wait();
            StartBelStockPoller();
            StartTradingViewPollers();

            StartKomBankPollers(logFile);
        }

        private void StartKomBankPollers(IMyLog logFile)
        {
            Task.Factory.StartNew( () => Model.KomBankListViewModel.Start(logFile));
        }

        private void StartTradingViewPollers()
        {
            Task.Factory.StartNew(() => new TradingViewPoller().
                Start(TradingViewTiker.EurUsd, Model.TradingViewVm.Rates.EurUsd, Model, 1000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.UsdRub, Model.TradingViewVm.Rates.UsdRub, Model, 1000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.EurRub, Model.TradingViewVm.Rates.EurRub, Model, 1000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.UkOil, Model.TradingViewVm.Rates.UkOil, Model, 2000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Gold, Model.TradingViewVm.Rates.Gold, Model, 2000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Spx, Model.TradingViewVm.Rates.SpSpx, Model, 3000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Voo, Model.TradingViewVm.Rates.AmexVoo, Model, 4000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Vix, Model.TradingViewVm.Rates.CboeVix, Model, 5000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Bnd, Model.TradingViewVm.Rates.AmexBnd, Model, 6000));
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