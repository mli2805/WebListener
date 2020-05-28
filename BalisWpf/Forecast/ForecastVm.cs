using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BalisStandard;
using BalisWpf.Annotations;

namespace BalisWpf
{
    public class ForecastVm : INotifyPropertyChanged
    {
        public double Usd { get; set; }
        public double Eur { get; set; }
        public double Rub { get; set; }

        public double UsdDelta { get; set; }
        public double EurDelta { get; set; }
        public double RubDelta { get; set; }

        public string UsdString => $"Usd  {Usd:#,0.0000}   ( {UsdDelta:+#,0.0000;-#,0.0000;0} / {(UsdDelta*100/Usd):+0.00;-0.00;0}% )";
        public string EurString => $"Eur  {Eur:#,0.0000}   ( {EurDelta:+#,0.0000;-#,0.0000;0} / {(EurDelta*100/Eur):+0.00;-0.00;0}% )";
        public string RubString => $"Rub  {Rub:#,0.0000}   ( {RubDelta:+#,0.0000;-#,0.0000;0} / {(RubDelta*100/Rub):+0.00;-0.00;0}% )";

        public string BasketString => CurrentNbRates == null || Basket.Equals(CurrentNbRates.Basket)
            ? $"{Basket:#,0.0000} "
            : $"{Basket:#,0.0000} ( {((Basket - CurrentNbRates.Basket)*10000):+#,0;-#,0;0} п / {((Basket/CurrentNbRates.Basket - 1)*100):+#,0.00;-#,0.00;0}% )"
            ;

        public List<string> ForecastList =>
            new List<string>()
            {
                "",
                UsdString,
                EurString,
                RubString,
                "",
                BasketString
            };

        public string Caption => "Прогноз";
        public NbRates CurrentNbRates;
        public double Basket;
       
        public void Initialize(NbRates currentNbRates)
        {
            CurrentNbRates = currentNbRates;
            Basket = CurrentNbRates.Basket;
        }

        public void CalculateNewRates(TradingViewRates forex)
        {
            if (CurrentNbRates == null) return;
            Usd = NbBasket.ForecastUsingForex(Basket, forex);
            Eur = Usd * forex.EurUsd.Lp;
            Rub = Usd / forex.UsdRub.Lp * 100; // в корзине курс за 1 рур , а храним за 100
            UsdDelta = Usd - CurrentNbRates.Usd;
            EurDelta = Eur - CurrentNbRates.Eur;
            RubDelta = Rub - CurrentNbRates.Rub;

            OnPropertyChanged("ForecastList");
        }

        private void CalculateNewRatesFromRub(TradingViewRates forex)
        {
            Usd = (Rub / 100) * forex.UsdRub.Lp;
            Eur = (Rub / 100) * forex.EurUsd.Lp * forex.UsdRub.Lp;
            Basket = NbBasket.Calculate(Usd, Eur, Rub / 100);

            UsdDelta = Usd - CurrentNbRates.Usd;
            EurDelta = Eur - CurrentNbRates.Eur;
            RubDelta = Rub - CurrentNbRates.Rub;
        }

        public void ForecastRatesFromAnotherBasket(double anotherBasket, TradingViewRates currentForex)
        {
            Basket = anotherBasket;
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
