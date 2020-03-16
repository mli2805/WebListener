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

            Task.Factory.StartNew(() => new TradingViewManager().TradingMain(TradingViewTiker.Voo, Model));
            Task.Factory.StartNew(() => new TradingViewManager().TradingMain(TradingViewTiker.EurUsd, Model));

        }

      

    }
}