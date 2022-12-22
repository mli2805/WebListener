using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BalisStandard;

namespace BalisWpf
{
    public class ForecastVm : INotifyPropertyChanged
    {
        public double Usd { get; set; }
        public double Eur { get; set; }
        public double Rub { get; set; }
        public double Cny { get; set; }

        public double UsdDelta { get; set; }
        public double EurDelta { get; set; }
        public double RubDelta { get; set; }
        public double CnyDelta { get; set; }

        public string UsdString => $"Usd  {Usd:#,0.0000}   ( {UsdDelta:+#,0.0000;-#,0.0000;0} / {(UsdDelta * 100 / Usd):+0.00;-0.00;0}% )";
        public string EurString => $"Eur  {Eur:#,0.0000}   ( {EurDelta:+#,0.0000;-#,0.0000;0} / {(EurDelta * 100 / Eur):+0.00;-0.00;0}% )";
        public string RubString => $"Rub  {Rub:#,0.0000}   ( {RubDelta:+#,0.0000;-#,0.0000;0} / {(RubDelta * 100 / Rub):+0.00;-0.00;0}% )";
        public string CnyString => $"Cny  {Cny:#,0.0000}   ( {CnyDelta:+#,0.0000;-#,0.0000;0} / {(CnyDelta * 100 / Cny):+0.00;-0.00;0}% )";

        public string BasketString => _currentNbRates == null || _basket.Equals(_currentNbRates.Basket)
            ? $"{_basket:#,0.0000} "
            : $"{_basket:#,0.0000} ( {((_basket - _currentNbRates.Basket) * 10000):+#,0;-#,0;0} п / {((_basket / _currentNbRates.Basket - 1) * 100):+#,0.00;-#,0.00;0}% )"
            ;

        public List<string> ForecastList =>
            new List<string>()
            {
                "",
                UsdString,
                EurString,
                RubString,
                CnyString,
                "",
                BasketString,
                "",
                "*используются для прогноза",

            };

        public string Caption => "Прогноз";
        private NbRates _currentNbRates;
        private double _basket;

        public void Initialize(NbRates currentNbRates)
        {
            _currentNbRates = currentNbRates;
            _basket = _currentNbRates.Basket;
        }

        public void CalculateNewRates(TradingViewRates forex)
        {
            if (_currentNbRates == null) return;
            Usd = NbBasket.ForecastUsdUsingForex(_basket, forex);
            Eur = Usd * forex.EurUsd.Lp;
            Rub = Usd / forex.InvUsdRub.Lp * 100; // в корзине курс за 1 рур , а храним за 100
            Cny = Usd / forex.UsdCny.Lp * 10; // в корзине курс за 1 юань , а храним за 10
            UsdDelta = Usd - _currentNbRates.Usd;
            EurDelta = Eur - _currentNbRates.Eur;
            RubDelta = Rub - _currentNbRates.Rub;
            CnyDelta = Cny - _currentNbRates.Cny;

            OnPropertyChanged(nameof(ForecastList));
        }

        private void CalculateNewRatesFromRub(TradingViewRates forex)
        {
            Usd = (Rub / 100) * forex.UsdRub.Lp;
            Eur = (Rub / 100) * forex.EurUsd.Lp * forex.UsdRub.Lp;
            Cny = (Rub / 10) * forex.UsdCny.Lp * forex.UsdRub.Lp;
            _basket = NbBasket.Calculate(Usd, Eur, Rub / 100, Cny / 10);

            UsdDelta = Usd - _currentNbRates.Usd;
            EurDelta = Eur - _currentNbRates.Eur;
            RubDelta = Rub - _currentNbRates.Rub;
        }

        public void ForecastRatesFromAnotherBasket(double anotherBasket, TradingViewRates currentForex)
        {
            _basket = anotherBasket;
            CalculateNewRates(currentForex);
        }

        public void ForecastRatesFromAnotherRub(TradingViewRates currentForex, double anotherRub)
        {
            Rub = anotherRub;
            CalculateNewRatesFromRub(currentForex);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
