using System;
using System.Threading;
using System.Threading.Tasks;

namespace BanksListener
{
    public class TradingViewManager
    {
        public void Poll()
        {
            Task.Factory.StartNew(() => Poll(TradingViewTiker.EurUsd));
            Task.Factory.StartNew(() => Poll(TradingViewTiker.UsdRub));
            Task.Factory.StartNew(() => Poll(TradingViewTiker.EurRub));
            Task.Factory.StartNew(() => Poll(TradingViewTiker.UkOil));
        }

        private async void Poll(TradingViewTiker tiker)
        {
            var extractor = new TradingViewExtractor();
            extractor.ResultFetched += ExtractorResultFetched;

            await extractor.ConnectWebSocket();
            await extractor.SessionRequested();
            await extractor.RequestRate(tiker);

            while (true)
            {
                var result = await extractor.ReceiveData();
                if (result)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    extractor = new TradingViewExtractor();
                    extractor.ResultFetched += ExtractorResultFetched;
                    await extractor.ConnectWebSocket();
                    await extractor.SessionRequested();
                    await extractor.RequestRate(tiker);
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void ExtractorResultFetched(object sender, TradingViewResult result)
        {
            Console.WriteLine($@"{DateTime.Now}  {result.Tiker}  {result.Value}");
            Console.WriteLine();
        }

//        private void ApplyRates(TradingViewResult result)
//        {
//            switch (result.Tiker)
//            {
//                case TradingViewTiker.EurUsd:
//                    Model.Forex.UsdEur = result.Value;
//                    break;
//                case TradingViewTiker.UsdRub:
//                    Model.Forex.RubUsd = result.Value;
//                    break;
//                case TradingViewTiker.EurRub:
//                    Model.Forex.EurRub = result.Value;
//                    break;
//                case TradingViewTiker.UkOil:
//                    Model.Forex.BrentUkOil = result.Value;
//                    break;
//            }
//
//            Model.Forex.LastChecked = DateTime.Now;
//            Model.Forecast = Model.Forecast == null
//                ? new ForecastRates(Model.NbRates1, Model.Forex)
//                : new ForecastRates(Model.NbRates1, Model.Forecast.Basket, Model.Forex);
//        }
    }
}