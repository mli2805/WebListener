using System;

namespace BalisStandard
{
    public static class ForecasterFromBasket
    {
        /// <summary>
        /// Basket = Usd^UsdWage * Eur^EurWage * Rub^RubWage * Cny^CnyWage
        /// где курсы валют это колво бинов за 1-цу валюты
        /// 
        /// из полученных на вход курсов на форексе выражаем валюты через доллар
        /// Basket = Usd^UsdWage * (Usd*Eur/Usd)^EurWage * (Usd*Rub/Usd)^RubWage * (Usd*Cny/Usd)^CnyWage
        /// Basket = Usd^UsdWage * Usd^EurWage * (Eur/Usd)^EurWage * Usd^RubWage * (Rub/Usd)^RubWage * Usd^CnyWage * (Cny/Usd)^CnyWage
        /// Basket / ( (Eur/Usd)^EurWage * (Rub/Usd)^RubWage * (Cny/Usd)^CnyWage ) = Usd^UsdWage * Usd^EurWage * Usd^RubWage * Usd^CnyWage
        /// Basket / ( (Eur/Usd)^EurWage * (Rub/Usd)^RubWage * (Cny/Usd)^CnyWage ) = Usd^(UsdWage + EurWage + RubWage + CnyWage)
        /// 
        /// веса по определению в сумме дают 1
        /// Basket / ( (Eur/Usd)^EurWage * (Rub/Usd)^RubWage * (Cny/Usd)^CnyWage ) = Usd
        /// 
        /// Forex.UsdRub & Forex.UsdCny - это на самом деле колво рублей или юаней за доллар, поэтому используем обратную величину
        /// </summary>
        /// <param name="currentBasket"></param>
        /// <param name="forex"></param>
        /// <param name="basketWeights"></param>
        /// <returns></returns>
        public static ForecastResult ForecastRatesUsingInvestingCom(double currentBasket, RatesForForecast forex,
           BasketWeights basketWeights)
        {
            var result = new ForecastResult();
            var usd = currentBasket /
                   (Math.Pow(forex.EurUsd, basketWeights.Euro)
                    * Math.Pow(1.0 / forex.UsdRub, basketWeights.Rur)
                    * Math.Pow(1.0 / forex.UsdCny, basketWeights.Cny));

            // бинов за валюту
            result.Usd = usd;
            result.Eur = usd * forex.EurUsd;
            result.Rub = usd / forex.UsdRub * 100; // при расчете курс бинов за 1 рур , а храним за 100
            result.Cny = usd / forex.UsdCny * 10; // при расчете курс бинов за 1 юань , а храним за 10

            return result;
        }


    }
}
