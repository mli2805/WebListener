namespace BalisWpf
{
    public class ShellVm
    {
        public NbRbViewModel NbRbViewModel { get; set; } = new NbRbViewModel();
        public BelStockViewModel BelStockViewModel { get; set; } = new BelStockViewModel();
        public TradingViewVm TradingViewVm { get; set; } = new TradingViewVm();
        public ForecastVm ForecastVm { get; set;  } = new ForecastVm();

        public KomBankListViewModel KomBankListViewModel { get; set; } = new KomBankListViewModel();
    }
}