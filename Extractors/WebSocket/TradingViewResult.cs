namespace Extractors
{
    public class TradingViewResult
    {
        public TradingViewChart Chart;
        public double Value; // Lp
        public double Pre; // pre-market
        public double Bid;
        public double Ask;

        public TradingViewResult()
        {
        }

        public TradingViewResult(string tikerName)
        {
            switch (tikerName)
            {
                case "AMEX:VOO": Chart = TradingViewChart.VOO; break;
            }
        }
    }
}