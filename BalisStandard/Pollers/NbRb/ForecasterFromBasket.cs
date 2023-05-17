using System;

namespace BalisStandard
{
    public static class ForecasterFromBasket
    {
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
        /// <param name="basketWeights"></param>
        /// <returns></returns>
        public static double ForecastUsdUsingForex(double currentBasket, TradingViewRates forexRates, BasketWeights basketWeights)
        {
            return currentBasket / 
                   (Math.Pow(forexRates.EurUsd.Lp, basketWeights.Euro) 
                    * Math.Pow(1.0 / forexRates.InvUsdRub.Lp, basketWeights.Rur) 
                    * Math.Pow(1.0 / forexRates.UsdCny.Lp, basketWeights.Cny));
        }
    }
}
