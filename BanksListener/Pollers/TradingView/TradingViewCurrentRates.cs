using System.Collections.Concurrent;

namespace BanksListener
{
    public class TradingViewCurrentRates
    {
        public ConcurrentDictionary<TradingViewTiker, double> Dict =
            new ConcurrentDictionary<TradingViewTiker, double>(4, 8);
    }
}