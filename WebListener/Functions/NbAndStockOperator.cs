using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using WebListener.DomainModel;
using WebListener.DomainModel.BelStock;
using WebListener.WebExtractorsAsync;

namespace WebListener.Functions
{
    class NbAndStockOperator
    {
        private readonly MainViewModel _vm;
        public NbAndStockOperator(MainViewModel vm)
        {
            _vm = vm;
        }
        public void Initialize()
        {
            InitializeBanki24Timer();
        }
        public async Task InitializeNbSection()
        {
            _vm.NbRates1 = await NbRbRatesDownloader.GetRatesForDate(DateTime.Today);
            _vm.NbRates0 = _vm.NbRates1;
            if (_vm.NbRates1.Usd < 0) return;
            while (_vm.NbRates1.Equals(_vm.NbRates0) && ((_vm.NbRates1.Date - _vm.NbRates0.Date).Days < 5) )
            {
                _vm.NbRates0 = await NbRbRatesDownloader.GetRatesForDate(_vm.NbRates0.Date.AddDays(-1));
            }
        }

        private NbRates ChooseStartNbRates(BelStock belStock)
        {
            switch (belStock.TradingState)
            {
                case BelStockState.HasNotStartedYet:
                    return _vm.NbRates0;
                case BelStockState.InProgress:
                    return _vm.NbRates1;
                default: // BelStockState.TerminatedAlready
                    return _vm.NbRates1.Date == belStock.TradingDate ? _vm.NbRates1 : _vm.NbRates0;
            }
        }
        public async Task<BelStockWrapped> GetWrappedCurrentStock()
        {
            var wrappedStock = new BelStockWrapped();
            var belStock = await new Banki24OnlineExtractor().GetStockAsync();
            wrappedStock.NbRates = ChooseStartNbRates(belStock);
            wrappedStock.BelStock = belStock;
            return wrappedStock;
        }
        private void InitializeBanki24Timer()
        {
            var timer = new DispatcherTimer();
            timer.Tick += Banki24TimerTick;
            timer.Interval = new TimeSpan(0, 0, 7);
            timer.Start();
        }
        private async void Banki24TimerTick(object sender, EventArgs e)
        {
            if (DateTime.Now.Hour < 10) return;
            if (_vm.NbRates1 != null && _vm.NbRates1.Date > DateTime.Today.Date) return;
            var temp = await new Banki24OnlineExtractor().GetStockAsync();
            if (temp == null) return;
            _vm.BelStockWrapped.BelStock = temp;
            if (_vm.BelStockWrapped.BelStock.TradingState == BelStockState.TerminatedAlready)
            {
                _vm.NbRates0 = _vm.NbRates1;
                _vm.NbRates1 = new NbRates{ Date = DateTime.Today.AddDays(1), Usd = _vm.BelStockWrapped.BelStock.Usd.Average,
                    Eur = _vm.BelStockWrapped.BelStock.Eur.Average, Rub = _vm.BelStockWrapped.BelStock.Rub.Average };
                _vm.Forecast = null;
            }
            if (_vm.BelStockWrapped.BelStock.TradingState == BelStockState.InProgress && _vm.BelStockWrapped.NbRates.Date != DateTime.Today)
            {
                _vm.BelStockWrapped.NbRates = _vm.NbRates1;
            }
        }
    }
}
