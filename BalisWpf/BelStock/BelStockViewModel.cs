using System.Collections.Generic;
using BalisStandard;
using Caliburn.Micro;

namespace BalisWpf
{
    public class BelStockViewModel : Screen
    {
        private readonly IWindowManager _windowManager;
        private readonly BelStockArchiveViewModel _belStockArchiveViewModel;

        public BelStockViewModel(IWindowManager windowManager, BelStockArchiveViewModel belStockArchiveViewModel)
        {
            _windowManager = windowManager;
            _belStockArchiveViewModel = belStockArchiveViewModel;
        }

        public NbRates PreviousTradeDayNbRates { get; set; } = new NbRates();
        public NbRates TodayNbRates { get; set; } = new NbRates();

        private BelStock _belStock = new BelStock();
        public BelStock BelStock
        {
            get => _belStock;
            set
            {
                if (Equals(value, _belStock)) return;
                _belStock = value;
                NotifyOfPropertyChange(nameof(Title));
                NotifyOfPropertyChange(nameof(StockToScreen));
            }
        }


        public List<string> StockToScreen =>
           new List<string>()
           {
               OneStockCurrencyTemplate(_belStock.Usd, PreviousTradeDayNbRates.Usd, TodayNbRates.Usd),
               OneStockCurrencyTemplate(_belStock.Eur, PreviousTradeDayNbRates.Eur, TodayNbRates.Eur),
               OneStockCurrencyTemplate(_belStock.Rub, PreviousTradeDayNbRates.Rub, TodayNbRates.Rub),
               "",
               BuildNewBasketString(),
               "",
               EurUsdString,
               UsdRubString,
               EurRubString,
           };

        private string OneStockCurrencyTemplate(BelStockCurrency currency, double previousNbRate, double todayNbRate)
        {
            return currency.Average.Equals(-1)
                ? $" вчерашний курс {todayNbRate}"
                : BelStock.TradingState == BelStockState.HasNotStartedYet
                    ? OneCurrencyTemplate(currency, previousNbRate)
                    : OneCurrencyTemplate(currency, todayNbRate);
        }

        private string OneCurrencyTemplate(BelStockCurrency currency, double nbrate)
        {
            var absoluteChanges = (currency.Average - nbrate).ToString("+0.0000;- 0.0000;0");
            var relativeChanges = ((currency.Average - nbrate) * 100 / nbrate).ToString("+0.00;- 0.00;0");
            var changes = $"({absoluteChanges} {relativeChanges}%)";
            return $"{currency.Average:#,0.0000} {changes}  {currency.Volume} / {currency.DealsCount:0.}  {currency.LastDeal:#,0.0000}";
        }

        public string Title
        {
            get
            {
                switch (_belStock.TradingState)
                {
                    case BelStockState.HasNotStartedYet:
                        return "результаты торгов " + _belStock.TradingDate.ToString("dd.MM");
                    case BelStockState.InProgress:
                        return "идут торги " + _belStock.LastChecked.ToString("dd.MM HH:mm:ss");
                    case BelStockState.TerminatedAlready:
                        return "по результатам " + _belStock.TradingDate.ToString("dd.MM");
                    default:
                        return "";
                }
            }
        }

        public string EurUsdString => _belStock.Eur.Average.Equals(-1) || _belStock.Usd.Average.Equals(-1) ? "" : $"{_belStock.Eur.Average / _belStock.Usd.Average:#,0.0000}";
        public string UsdRubString => _belStock.Usd.Average.Equals(-1) || _belStock.Rub.Average.Equals(-1) ? "" : $"{_belStock.Usd.Average * 100 / _belStock.Rub.Average:#,0.00}";
        public string EurRubString => _belStock.Eur.Average.Equals(-1) || _belStock.Rub.Average.Equals(-1) ? "" : $"{_belStock.Eur.Average * 100 / _belStock.Rub.Average:#,0.00}";
        private double CalculateNewBasket()
        {
            var usd = _belStock.Usd.Average.Equals(-1) ? TodayNbRates.Usd : _belStock.Usd.Average;
            var eur = _belStock.Eur.Average.Equals(-1) ? TodayNbRates.Eur : _belStock.Eur.Average;
            var rub = _belStock.Rub.Average.Equals(-1) ? TodayNbRates.Rub : _belStock.Rub.Average;
            return NbBasket.Calculate(usd, eur, rub / 100);
        }
        private string BuildNewBasketString()
        {
            var oldBasket = _belStock.TradingState == BelStockState.HasNotStartedYet
                ? PreviousTradeDayNbRates.Basket
                : TodayNbRates.Basket;
            var newBasket = CalculateNewBasket();
            return $"{newBasket:#,0.0000} " +
                   $"( {((newBasket - oldBasket) * 10000):+0.00;-0.00} п / {((newBasket - oldBasket) * 100 / newBasket):+0.00;-0.00}% ) ";
        }


        public async void ShowBelStockArchive()
        {
            await _belStockArchiveViewModel.Initialize();
            _windowManager.ShowWindow(_belStockArchiveViewModel);
        }

    }
}
