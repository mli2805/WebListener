using System.ComponentModel;
using System.Runtime.CompilerServices;
using BalisStandard;
using BalisWpf.Annotations;

namespace BalisWpf
{
    public class BelStockVm : INotifyPropertyChanged
    {
        public NbRates NbRates { get; set; } = new NbRates();

        private BelStock _belStock = new BelStock();
        public BelStock BelStock
        {
            get => _belStock;
            set
            {
                if (Equals(value, _belStock)) return;
                _belStock = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UsdString));
                OnPropertyChanged(nameof(EurString));
                OnPropertyChanged(nameof(RubString));
                OnPropertyChanged(nameof(NewBasketString));
                OnPropertyChanged(nameof(EurUsdString));
                OnPropertyChanged(nameof(EurRubString));
                OnPropertyChanged(nameof(UsdRubString));
                OnPropertyChanged(nameof(Title));
            }
        }

        public string DuringTradingSessionTemplate(BelStockCurrency currency, double nbrate)
        {
            var absoluteChanges = (currency.Average - nbrate).ToString("+0.0000;- 0.0000;0");
            var relativeChanges = ((currency.Average - nbrate) * 100 / nbrate).ToString("+0.00;- 0.00;0");
            var changes = $"({absoluteChanges} {relativeChanges}%)";
            return $"{currency.Average:#,0.0000} {changes}  {currency.Volume} млн {currency.LastDeal:#,0.0000}";
        }

        public string UsdString => _belStock.Usd.Average.Equals(-1)
            ? $" вчерашний курс {NbRates.Usd}"
            : DuringTradingSessionTemplate(_belStock.Usd, NbRates.Usd);

        public string EurString => _belStock.Eur.Average.Equals(-1)
            ? $" вчерашний курс {NbRates.Eur}"
            : DuringTradingSessionTemplate(_belStock.Eur, NbRates.Eur);

        public string RubString => _belStock.Rub.Average.Equals(-1)
            ? $" вчерашний курс {NbRates.Rub}"
            : DuringTradingSessionTemplate(_belStock.Rub, NbRates.Rub);

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

        private double _basketChangesDelta;
        private double _basketChangesCurrent;
        public string NewBasketString => BuildNewBasketString();
        public string EurUsdString => _belStock.Eur.Average.Equals(-1) || _belStock.Usd.Average.Equals(-1) ? "" : $"{_belStock.Eur.Average / _belStock.Usd.Average:#,0.0000}";
        public string UsdRubString => _belStock.Usd.Average.Equals(-1) || _belStock.Rub.Average.Equals(-1) ? "" : $"{_belStock.Usd.Average * 100 / _belStock.Rub.Average:#,0.00}";
        public string EurRubString => _belStock.Eur.Average.Equals(-1) || _belStock.Rub.Average.Equals(-1) ? "" : $"{_belStock.Eur.Average * 100 / _belStock.Rub.Average:#,0.00}";
        private double CalculateNewBasket()
        {
            var usd = _belStock.Usd.Average.Equals(-1) ? NbRates.Usd : _belStock.Usd.Average;
            var eur = _belStock.Eur.Average.Equals(-1) ? NbRates.Eur : _belStock.Eur.Average;
            var rub = _belStock.Rub.Average.Equals(-1) ? NbRates.Rub : _belStock.Rub.Average;
            return NbBasket.Calculate(usd, eur, rub / 100);
        }
        private string BuildNewBasketString()
        {
            var newBasket = CalculateNewBasket();
            var basketChanges = (newBasket - NbRates.Basket) * 10000;
            if (!basketChanges.Equals(_basketChangesCurrent))
            {
                if (!_basketChangesCurrent.Equals(0)) _basketChangesDelta = basketChanges - _basketChangesCurrent;
                _basketChangesCurrent = basketChanges;
            }
            return $"{newBasket:#,0.0000} " +
                   $"( {((newBasket - NbRates.Basket) * 10000):+0.00;-0.00} п / {((newBasket - NbRates.Basket) * 100 / newBasket):+0.00;-0.00}% ) " +
                   $"{_basketChangesDelta:+#,0.00;-#,0.00} п";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}