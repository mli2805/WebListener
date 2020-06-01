using System;

namespace BalisStandard
{
    public class BelStock
    {
        public BelStockState TradingState { get; set; }
        public DateTime TradingDate { get; set; }
        public DateTime LastChecked { get; set; }

        public BelStockCurrency Usd { get; set; }

        public BelStockCurrency Eur { get; set; }

        public BelStockCurrency Rub { get; set; }

        public BelStock()
        {
            Usd = new BelStockCurrency();
            Eur = new BelStockCurrency();
            Rub = new BelStockCurrency();
        }

        public NbRates GetTomorrow()
        {
            return new NbRates()
            {
                Date= TradingDate.AddDays(1),
                Usd = Usd.Average,
                Eur = Eur.Average,
                Rub = Rub.Average,
            };
        }
    }
}