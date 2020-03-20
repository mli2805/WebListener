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

            StartNbRbExtractor();
            StartTradingViewExtractors();
        }

        private void StartTradingViewExtractors()
        {
            Task.Factory.StartNew(() => new TradingViewManager().
                Start(TradingViewTiker.EurUsd, Model.TradingViewData.EurUsd, Model, 1000));
            Task.Factory.StartNew(() => new TradingViewManager().Start(TradingViewTiker.UsdRub, Model.TradingViewData.UsdRub,  Model, 1000));
            Task.Factory.StartNew(() => new TradingViewManager().Start(TradingViewTiker.EurRub, Model.TradingViewData.EurRub,  Model, 1000));
            Task.Factory.StartNew(() => new TradingViewManager().Start(TradingViewTiker.UkOil,  Model.TradingViewData.UkOil,   Model, 2000));
            Task.Factory.StartNew(() => new TradingViewManager().Start(TradingViewTiker.Gold,   Model.TradingViewData.Gold,    Model, 2000));
            Task.Factory.StartNew(() => new TradingViewManager().Start(TradingViewTiker.Spx,    Model.TradingViewData.SpSpx,   Model, 3000));
            Task.Factory.StartNew(() => new TradingViewManager().Start(TradingViewTiker.Voo,    Model.TradingViewData.AmexVoo, Model, 4000));
            Task.Factory.StartNew(() => new TradingViewManager().Start(TradingViewTiker.Vix,    Model.TradingViewData.CboeVix, Model, 5000));
            Task.Factory.StartNew(() => new TradingViewManager().Start(TradingViewTiker.Bnd,    Model.TradingViewData.AmexBnd, Model, 6000));
        }

        private void StartNbRbExtractor()
        {
            Task.Factory.StartNew(() => new NbRbPoller().Start(Model));
        }

    }
}