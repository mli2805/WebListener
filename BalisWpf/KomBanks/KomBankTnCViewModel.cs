using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BalisStandard;
using Caliburn.Micro;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using UtilsLib;

namespace BalisWpf
{
    public class KomBankTnCViewModel : Screen
    {
        private readonly KomBankE _komBankE;
        private readonly IniFile _iniFile;
        private readonly IMyLog _logFile;
        private readonly IWindowManager _windowManager;
        private readonly ChangesViewModel _changesViewModel;
        public KomBankViewModel KomBankViewModel { get; set; }
        public PlotModel MyPlotModel { get; set; } = new PlotModel();

        public KomBankTnCViewModel(KomBankE komBankE, IniFile iniFile, IMyLog logFile, IWindowManager windowManager, ChangesViewModel changesViewModel)
        {
            _komBankE = komBankE;
            _iniFile = iniFile;
            _logFile = logFile;
            _windowManager = windowManager;
            _changesViewModel = changesViewModel;
        }

        protected override void OnViewLoaded(object view)
        {
            DisplayName = _komBankE.GetAbbreviation();
        }

        public async Task Initialize()
        {
            KomBankViewModel = await new KomBankViewModel(_iniFile, _komBankE, _logFile, _windowManager, _changesViewModel).GetSomeLast();
            InitializeChart();
        }

        private void InitializeChart()
        {
            MyPlotModel.Axes.Add(new DateTimeAxis()
            {
                IntervalType = DateTimeIntervalType.Auto,
                StringFormat = @"dd\/MM HH:mm",
                MajorGridlineStyle = LineStyle.DashDot,
                Position = AxisPosition.Bottom,
            });

            MyPlotModel.Axes.Add(new LinearAxis(){ Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Dash});

            var lineSeriesA = new LineSeries() { Title = "sell", Color = OxyColors.Green };
            var lineSeriesB = new LineSeries() { Title = "buy", Color = OxyColors.Blue };

            foreach (var point in Convert(KomBankViewModel.Rows, "usd"))
            {
                lineSeriesA.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.Timestamp), point.Rates.Item1));
                lineSeriesB.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.Timestamp), point.Rates.Item2));
            }

            MyPlotModel.Series.Add(lineSeriesA);
            MyPlotModel.Series.Add(lineSeriesB);
        }

        private List<OneCurrencyChartDataLine> Convert(IEnumerable<KomBankRateVm> vms, string currency)
        {
            var result = new List<OneCurrencyChartDataLine>();
            Tuple<double, double> prevRates = null;
            foreach (var vm in vms)
            {
                var rates = vm.GetCurrency(currency);
                if (prevRates != null)
                    result.Add(new OneCurrencyChartDataLine(vm.StartedFrom.AddMinutes(-1), prevRates));
                result.Add(new OneCurrencyChartDataLine(vm.StartedFrom, rates));
                prevRates = rates;
            }
            return result;
        }
    }

    public class OneCurrencyChartDataLine
    {
        public DateTime Timestamp;
        public Tuple<double, double> Rates;

        public OneCurrencyChartDataLine(DateTime timestamp, Tuple<double, double> rates)
        {
            Timestamp = timestamp;
            Rates = rates;
        }
    }
}
