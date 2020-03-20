using System.Threading.Tasks;
using BalisStandard;

namespace BalisWpf 
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        public ShellVm Model { get; set; }

        public ShellViewModel()
        {
            Model = new ShellVm(){Test = "in C-tor"};

            StartNbRbPoller();
            StartBelStockPoller();
            StartTradingViewPoller();
        }

        private void StartTradingViewPoller()
        {
            Task.Factory.StartNew(() => new TradingViewPoller().
                Start(TradingViewTiker.EurUsd, Model.TradingViewData.EurUsd, Model, 1000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.UsdRub, Model.TradingViewData.UsdRub,  Model, 1000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.EurRub, Model.TradingViewData.EurRub,  Model, 1000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.UkOil,  Model.TradingViewData.UkOil,   Model, 2000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Gold,   Model.TradingViewData.Gold,    Model, 2000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Spx,    Model.TradingViewData.SpSpx,   Model, 3000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Voo,    Model.TradingViewData.AmexVoo, Model, 4000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Vix,    Model.TradingViewData.CboeVix, Model, 5000));
            Task.Factory.StartNew(() => new TradingViewPoller().Start(TradingViewTiker.Bnd,    Model.TradingViewData.AmexBnd, Model, 6000));
        }

        private void StartNbRbPoller()
        {
            Task.Factory.StartNew(() => new NbRbPoller().Start(Model));
        }

        private void StartBelStockPoller() { }

    }
}