using System;
using System.Windows.Threading;
using WebListener.DomainModel;
using WebListener.Functions;

namespace WebListener.WebExtractors.Tradingview
{
    public class TradingviewExtractorsPool
    {
        private readonly MainViewModel _vm;
        private Forex _previousForex;

        private TradingviewExtractor _tradingviewExtractorEurUsd;
        private TradingviewExtractor _tradingviewExtractorUsdRub;
        private TradingviewExtractor _tradingviewExtractorEurRub;
        private TradingviewExtractor _tradingviewExtractorBrent;
        public TradingviewExtractorsPool(MainViewModel vm)
        {
            _vm = vm;
            _previousForex = null;
        }

        public void Start()
        {
            InitializeTradingviewExtractors();
            InitializeTradingviewTimer();
        }
        void TradingviewExtractorUsdRubCrossRateFetched(object sender, CrossRateFetchedEventsArgs e)
        {
            lock (_vm.Forex)
            {
                _vm.Forex.LastChecked = DateTime.Now;
                _vm.Forex.RubUsd = e.Rate;
                _vm.Forecast = _vm.Forecast == null
                    ? new ForecastRates(_vm.NbRates1, _vm.Forex)
                    : new ForecastRates(_vm.NbRates1, _vm.Forecast.Basket, _vm.Forex);
            }

        }
        void TradingviewExtractorEurUsdCrossRateFetched(object sender, CrossRateFetchedEventsArgs e)
        {
            lock (_vm.Forex)
            {
                _vm.Forex.LastChecked = DateTime.Now;
                _vm.Forex.UsdEur = e.Rate;
                _vm.Forecast = _vm.Forecast == null
                    ? new ForecastRates(_vm.NbRates1, _vm.Forex)
                    : new ForecastRates(_vm.NbRates1, _vm.Forecast.Basket, _vm.Forex);
            }
        }
        void TradingviewExtractorEurRubCrossRateFetched(object sender, CrossRateFetchedEventsArgs e)
        {
            lock (_vm.Forex)
            {
                _vm.Forex.LastChecked = DateTime.Now;
                _vm.Forex.EurRub = e.Rate;
            }
        }
        private void TradingviewExtractorBrentCrossRateFetched(object sender, CrossRateFetchedEventsArgs e)
        {
            lock (_vm.Forex)
            {
                _vm.Forex.LastChecked = DateTime.Now;
                _vm.Forex.BrentUkOil = e.Rate;

                if (_vm.Forex.IsRatherDifferent(_previousForex))
                {
                    _previousForex = _vm.Forex;
                    new KomBankFileOperator().SaveForex(_previousForex);
                }
            }
        }

        private void InitializeTradingviewExtractors()
        {
            _tradingviewExtractorEurUsd = new TradingviewExtractor("EURUSD");
            _tradingviewExtractorEurUsd.CrossRateFetched += TradingviewExtractorEurUsdCrossRateFetched;
            _tradingviewExtractorUsdRub = new TradingviewExtractor("USDRUB");
            _tradingviewExtractorUsdRub.CrossRateFetched += TradingviewExtractorUsdRubCrossRateFetched;
            _tradingviewExtractorEurRub = new TradingviewExtractor("EURRUB");
            _tradingviewExtractorEurRub.CrossRateFetched += TradingviewExtractorEurRubCrossRateFetched;
            _tradingviewExtractorBrent = new TradingviewExtractor("UKOIL");
            _tradingviewExtractorBrent.CrossRateFetched += TradingviewExtractorBrentCrossRateFetched;
        }

        private void InitializeTradingviewTimer()
        {
            _vm.Forex = new Forex();
            TradingviewTimerTick(this, null);
            var timer = new DispatcherTimer();
            timer.Tick += TradingviewTimerTick;
            timer.Interval = new TimeSpan(0, 0, 7);
            timer.Start();
        }

        void TradingviewTimerTick(object sender, EventArgs e)
        {
            Console.WriteLine(@"TradingviewTimerTick: ");
            _tradingviewExtractorEurUsd.GetRate();
            _tradingviewExtractorUsdRub.GetRate();
            _tradingviewExtractorEurRub.GetRate();
            _tradingviewExtractorBrent.GetRate();
        }
    }
}
