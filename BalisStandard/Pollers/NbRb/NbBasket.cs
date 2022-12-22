using System;

namespace BalisStandard
{
    public static class NbBasket
    {
        /* c 01/01/2016
        private const double UsdWage = 0.3;
        private const double EurWage = 0.3;
        private const double RubWage = 0.4;
        */

        /* c 01/11/2016
        private const double UsdWage = 0.3;
        private const double EurWage = 0.2;
        private const double RubWage = 0.5;
        */

        /* c 15/07/2022
        private const double UsdWage = 0.3;
        private const double EurWage = 0.1;
        private const double RubWage = 0.5;
        private const double CnyWage = 0.1;
        */

        // c 12/12/2022
        private const double UsdWage = 0.3;
        private const double EurWage = 0;
        private const double RubWage = 0.6;
        private const double CnyWage = 0.1;



        public static double Calculate(double usd, double eur, double rub, double cny)
        {
            return Math.Pow(usd, UsdWage) * Math.Pow(eur, EurWage) * Math.Pow(rub, RubWage) * Math.Pow(cny, CnyWage);
        }

        /// <summary>
        /// Basket = Usd^UsdWage * Eur^EurWage * Rub^RubWage
        /// где курсы валют это колво бинов за 1-цу валюты
        /// 
        /// из полученных на вход курсов на форексе выражаем валюты через доллар
        /// Basket = Usd^UsdWage * (Usd*Eur/Usd)^EurWage * (Usd*Rub/Usd)^RubWage
        /// Basket = Usd^UsdWage * Usd^EurWage * (Eur/Usd)^EurWage * Usd^RubWage * (Rub/Usd)^RubWage
        /// Basket / ( (Eur/Usd)^EurWage * (Rub/Usd)^RubWage) = Usd^UsdWage * Usd^EurWage * Usd^RubWage
        /// Basket / ( (Eur/Usd)^EurWage * (Rub/Usd)^RubWage) = Usd^(UsdWage + EurWage + RubWage)
        /// 
        /// веса по определению в сумме дают 1
        /// Basket / ( (Eur/Usd)^EurWage * (Rub/Usd)^RubWage) = Usd
        /// 
        /// Forex.RubUsd - это на самом деле колво рублей за доллар, поэтому используем обратную величину
        /// </summary>
        /// <param name="currentBasket"></param>
        /// <param name="forexRates"></param>
        /// <returns></returns>
        public static double ForecastUsdUsingForex(double currentBasket, TradingViewRates forexRates)
        {
            return currentBasket / 
                   (Math.Pow(forexRates.EurUsd.Lp, EurWage) 
                    * Math.Pow(1.0 / forexRates.InvUsdRub.Lp, RubWage) 
                    * Math.Pow(1.0 / forexRates.UsdCny.Lp, CnyWage));
        }
    }
}
