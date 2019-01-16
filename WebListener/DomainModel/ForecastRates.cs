namespace WebListener.DomainModel
{
    public class ForecastRates
    {
        public double Usd { get; set; }
        public double Eur { get; set; }
        public double Rub { get; set; }

        public double UsdDelta { get; set; }
        public double EurDelta { get; set; }
        public double RubDelta { get; set; }

        public string UsdString => $"Usd  {Usd:#,0.0000}   ( {UsdDelta.ToString("+#,0.0000;-#,0.0000;0")} / {(UsdDelta*100/Usd).ToString("+0.00;-0.00;0")}% )";
        public string EurString => $"Eur  {Eur:#,0.0000}   ( {EurDelta.ToString("+#,0.0000;-#,0.0000;0")} / {(EurDelta*100/Eur).ToString("+0.00;-0.00;0")}% )";
        public string RubString => $"Rub  {Rub:#,0.0000}   ( {RubDelta.ToString("+#,0.0000;-#,0.0000;0")} / {(RubDelta*100/Rub).ToString("+0.00;-0.00;0")}% )";

        public string BasketString => Basket.Equals(_currentNbRates.Basket)
            ? $"{Basket:#,0.0000} "
            : $"{Basket:#,0.0000} ( {((Basket - _currentNbRates.Basket)*10000).ToString("+#,0;-#,0;0")} п / {((Basket/_currentNbRates.Basket - 1)*100).ToString("+#,0.00;-#,0.00;0")}% )"
            ;

        private readonly NbRates _currentNbRates;
        private readonly Forex _currentForex;
        public double Basket;
        public ForecastRates(NbRates currentNbRates, Forex currentForex)
        {
            _currentNbRates = currentNbRates;
            _currentForex = currentForex;
            Basket = _currentNbRates.Basket;

            CalculateNewRates();
        }

        private void CalculateNewRates()
        {
            Usd = NbBasket.ForecastUsingForex(Basket, _currentForex);
            Eur = Usd * _currentForex.UsdEur;
            Rub = Usd / _currentForex.RubUsd * 100; // в корзине курс за 1 рур , а храним за 100
            UsdDelta = Usd - _currentNbRates.Usd;
            EurDelta = Eur - _currentNbRates.Eur;
            RubDelta = Rub - _currentNbRates.Rub;
        }

        private void CalculateNewRatesFromRub()
        {
            Usd = (Rub / 100) * _currentForex.RubUsd;
            Eur = (Rub / 100) * _currentForex.UsdEur*_currentForex.RubUsd;
            Basket = NbBasket.Calculate(Usd, Eur, Rub / 100);

            UsdDelta = Usd - _currentNbRates.Usd;
            EurDelta = Eur - _currentNbRates.Eur;
            RubDelta = Rub - _currentNbRates.Rub;
        }

        public ForecastRates(NbRates currentNbRates, double anotherBasket, Forex currentForex)
        {
            _currentNbRates = currentNbRates;
            _currentForex = currentForex;
            Basket = anotherBasket;

            CalculateNewRates();
        }

        public ForecastRates(NbRates currentNbRates, Forex currentForex, double anotherRub)
        {
            _currentNbRates = currentNbRates;
            _currentForex = currentForex;
            Rub = anotherRub;

            CalculateNewRatesFromRub();
        }
    }
}
