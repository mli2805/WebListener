using System.Windows;
using System.Windows.Controls;

namespace WebListener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainViewModel Vm { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = "Web Listener";
            Vm = new MainViewModel();
            Vm.Forex = new Forex();
            new KomBankFileOperator().LoadBanksHistory(Vm);

            var manager = new NbAndStockOperator(Vm);
            await manager.InitializeNbSection();
            Vm.BelStockWrapped = await manager.GetWrappedCurrentStock();
            manager.Initialize();
            Vm.Calculations = new Calculations(10.4, 1, Vm.NbRates1.Usd);

            new TradingViewManager().Start(Vm);
            new EcopressOperator(Vm).Start();

            new KomBankOperator(Vm).InitializeKomBanksTimers();
            new BpsOmcOperator(Vm).InitializeOmcTimer();
        }

        #region buttons' click handlers
        private void ShowBelStockArchieve(object sender, RoutedEventArgs e)
        {
            var belStockArchieveView = new BelStockArchieveView();
            belStockArchieveView.Show();
        }

        private void ButtonForecastPlus(object sender, RoutedEventArgs e)
        {
            var newBasket = Vm.Forecast.Basket + 0.0001;
            Vm.Forecast = new ForecastRates(Vm.NbRates1, newBasket, Vm.Forex);
        }
        private void ButtonForecastMinus(object sender, RoutedEventArgs e)
        {
            var newBasket = Vm.Forecast.Basket - 0.0001;
            Vm.Forecast = new ForecastRates(Vm.NbRates1, newBasket, Vm.Forex);
        }
        private void ButtonForecastOnBasket(object sender, RoutedEventArgs e)
        {
            var newBasket = Vm.NbRates1.Basket;
            Vm.Forecast = new ForecastRates(Vm.NbRates1, newBasket, Vm.Forex);
        }
        private void ButtonForecastOnRub(object sender, RoutedEventArgs e)
        {
            var newRub = Vm.NbRates1.Rub;
            Vm.Forecast = new ForecastRates(Vm.NbRates1, Vm.Forex, newRub);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((DataGrid)sender).UnselectAllCells();
        }

        #endregion

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // TODO bank's visibility menu
        }
    }
}
