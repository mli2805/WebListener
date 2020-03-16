using System.Collections.Concurrent;

namespace BalisStandard
{
    public class TradingViewCurrentRates
    {
        public ConcurrentDictionary<TradingViewTiker, double> Dict =
            new ConcurrentDictionary<TradingViewTiker, double>(4, 8);
    }
}