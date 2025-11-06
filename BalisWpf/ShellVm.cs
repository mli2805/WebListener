namespace BalisWpf
{
    public class ShellVm
    {
        public NbRbViewModel NbRbViewModel { get; set; } = new NbRbViewModel();
        public BelStockViewModel BelStockViewModel { get; set; }

        public ForexViewModel ForexViewModel { get; set; } = new ForexViewModel();
        public ForecastVm ForecastVm { get; set;  } = new ForecastVm();

        public KomBankListViewModel KomBankListViewModel { get; set; } = new KomBankListViewModel();

        public ShellVm(BelStockViewModel belStockViewModel)
        {
            BelStockViewModel = belStockViewModel;
        }
    }
}