using System;

namespace BalisLibrary
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
    }
}