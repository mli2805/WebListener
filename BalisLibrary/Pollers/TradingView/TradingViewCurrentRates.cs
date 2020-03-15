using System.Collections.Concurrent;

namespace BalisLibrary
{
    public class TradingViewCurrentRates
    {
        public ConcurrentDictionary<TradingViewTiker, double> Dict =
            new ConcurrentDictionary<TradingViewTiker, double>(4, 8);
    }
}