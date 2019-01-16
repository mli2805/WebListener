using Caliburn.Micro;

namespace WebSocketWpf
{
    public class ShellViewModel : Screen, IShell
    {
        public TradingViewVm TradingViewVm { get; set; } = new TradingViewVm();

        protected override void OnViewLoaded(object view)
        {
            DisplayName = "WebSocket";
            new TradingViewManager().Start(TradingViewVm);
        }
    }
}