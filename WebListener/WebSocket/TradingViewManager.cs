using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WebListener.DomainModel;

namespace WebListener
{
    public class TradingViewManager
    {
        //    public TradingViewVm TradingViewVm { get; set; }

        public MainViewModel Model { get; set; }

        public void Start(MainViewModel model)
        {
            Model = model;

            Task.Factory.StartNew(() => StartExtr(TradingViewChart.EurUsd));
            Task.Factory.StartNew(() => StartExtr(TradingViewChart.UsdRub));
            Task.Factory.StartNew(() => StartExtr(TradingViewChart.EurRub));
            Task.Factory.StartNew(() => StartExtr(TradingViewChart.UkOil));
        }

        private async void StartExtr(TradingViewChart chart)
        {
            var extractor = new TradingViewExtractor();
            extractor.ResultFetched += ExtractorResultFetched;

            await extractor.ConnectWebSocket();
            await extractor.SessionRequested();
            await extractor.RequestRate(chart);

            while (true)
            {
                var result = await extractor.ReceiveData();
                if (result)
                {
                    Thread.Sleep(10);
                }
                else
                {
                    extractor = new TradingViewExtractor();
                    extractor.ResultFetched += ExtractorResultFetched;
                    await extractor.ConnectWebSocket();
                    await extractor.SessionRequested();
                    await extractor.RequestRate(chart);
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void ExtractorResultFetched(object sender, TradingViewResult result)
        {
            Console.WriteLine($@"{DateTime.Now}  {result.Chart}  {result.Value}");
            Console.WriteLine();
            Application.Current.Dispatcher.Invoke(() => ApplyRates(result));
        }

        private void ApplyRates(TradingViewResult result)
        {
            switch (result.Chart)
            {
                case TradingViewChart.EurUsd:
                    //  TradingViewVm.EurUsdRate = result.Value.ToString("##0.00000");
                    Model.Forex.UsdEur = result.Value;
                    break;
                case TradingViewChart.UsdRub:
                    Model.Forex.RubUsd = result.Value;
                    //   TradingViewVm.UsdRub = result.Value;
                    break;
                case TradingViewChart.EurRub:
                    Model.Forex.EurRub = result.Value;
                    //  TradingViewVm.EurRubRate = result.Value.ToString("##0.0000");
                    break;
                case TradingViewChart.UkOil:
                    //                    TradingViewVm.Brent = result.Value;
                    Model.Forex.BrentUkOil = result.Value;
                    break;
            }

            Model.Forex.LastChecked = DateTime.Now;
            Model.Forecast = Model.Forecast == null
                ? new ForecastRates(Model.NbRates1, Model.Forex)
                : new ForecastRates(Model.NbRates1, Model.Forecast.Basket, Model.Forex);
        }
    }
}