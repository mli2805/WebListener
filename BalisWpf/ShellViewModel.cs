using System.Threading.Tasks;
using BalisStandard;

namespace BalisWpf
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        public ShellVm Model { get; set; }

        public ShellViewModel()
        {
            Model = new ShellVm() { Test = "in C-tor" };

            StartNbRbPoller();
            StartBelStockPoller();
            StartTradingViewPoller();
        }

        private void StartTradingViewPoller()
        {
            Task.Factory.StartNew(() => new TradingViewPoller().
                Start(TradingViewTiker.EurUsd, Model.TradingViewVm.EurUsd, Model, 1000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.UsdRub, Model.TradingViewVm.UsdRub, Model, 1000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.EurRub, Model.TradingViewVm.EurRub, Model, 1000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.UkOil, Model.TradingViewVm.UkOil, Model, 2000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Gold, Model.TradingViewVm.Gold, Model, 2000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Spx, Model.TradingViewVm.SpSpx, Model, 3000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Voo, Model.TradingViewVm.AmexVoo, Model, 4000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Vix, Model.TradingViewVm.CboeVix, Model, 5000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Bnd, Model.TradingViewVm.AmexBnd, Model, 6000));
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