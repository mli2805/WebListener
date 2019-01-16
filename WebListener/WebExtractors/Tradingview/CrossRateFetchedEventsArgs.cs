using System;

namespace WebListener.WebExtractors.Tradingview
{
    public class CrossRateFetchedEventsArgs : EventArgs
    {
        public double Rate { get; set; }
    }
}