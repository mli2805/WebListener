using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using UtilsLib;

namespace BalisWpf
{
    public class MonthlyChartViewModel : Screen
    {
        private ObservableCollection<BelStockArchiveLine> Rates { get; set; } = new ObservableCollection<BelStockArchiveLine>();

        public PlotModel MyPlotModel { get; set; } = new PlotModel();
        public PlotModel MyPlotModel2 { get; set; } = new PlotModel();

        protected override void OnViewLoaded(object view)
        {
            DisplayName = "Курс бин к корзине валют в целом и сумарный оборот на бирже по дням месяца";
        }

        public void Initialize(ObservableCollection<BelStockArchiveLine> rows)
        {
            Rates = rows;
            var start = DateTime.Today.AddMonths(-5).GetStartOfMonth();
            var lines = rows.Where(l => l.Date.Date >= start.Date).ToList();
            var min = lines.Min(l => l.Basket) * 0.999;
            var max = lines.Max(l => l.Basket) * 1.001;
            var maxt = lines.Max(l => l.TotalTurnover) * 1.001;

            OneMonth(start.AddMonths(0), MyPlotModel, OxyColor.FromArgb(255, 0, 0, 255));
            OneMonth(start.AddMonths(1), MyPlotModel, OxyColor.FromArgb(255, 0, 255, 0));
            OneMonth(start.AddMonths(2), MyPlotModel, OxyColor.FromArgb(255, 255, 0, 0));
            OneMonth(start.AddMonths(3), MyPlotModel2, OxyColor.FromArgb(255, 0, 0, 255));
            OneMonth(start.AddMonths(4), MyPlotModel2, OxyColor.FromArgb(255, 0, 255, 0));
            OneMonth(start.AddMonths(5), MyPlotModel2, OxyColor.FromArgb(255, 255, 0, 0));

            MyPlotModel.Axes.Add(new LinearAxis() { Key = "basketAxis", Position = AxisPosition.Left, Minimum = min, Maximum = max });
            MyPlotModel.Axes.Add(new LinearAxis() { Key = "turnoverAxis", Position = AxisPosition.Right, Maximum = maxt });

            MyPlotModel2.Axes.Add(new LinearAxis() { Key = "basketAxis", Position = AxisPosition.Left, Minimum = min, Maximum = max });
            MyPlotModel2.Axes.Add(new LinearAxis() { Key = "turnoverAxis", Position = AxisPosition.Right, Maximum = maxt });
        }


        private void OneMonth(DateTime date, PlotModel plotModel, OxyColor color)
        {
            var year = date.Year;
            var month = date.Month;
            var lineSeriesBasket = new LineSeries() { Title = $"{month} {year}", Color = color, YAxisKey = "basketAxis" };
            var columnSeriesTurnover = new ColumnSeries() { FillColor = OxyColor.FromAColor(64, color), YAxisKey = "turnoverAxis" };

            var lastDayOfPrevMonth = Rates.Last(r => r.Date < new DateTime(year, month, 1));
            lineSeriesBasket.Points.Add(new DataPoint(0, lastDayOfPrevMonth.Basket));
            var prevDay = lastDayOfPrevMonth;

            int lastDayInMonth;
            if (DateTime.Now.Year == year && DateTime.Now.Month == month)
                lastDayInMonth = Rates.FirstOrDefault(r=>r.Date == DateTime.Today) != null 
                    ? DateTime.Today.Day 
                    : DateTime.Today.AddDays(-1).Day;
            else lastDayInMonth = DateTime.DaysInMonth(year, month);
            for (int i = 1; i <= lastDayInMonth; i++)
            {
                var day = Rates.FirstOrDefault(r => r.Date == new DateTime(year, month, i));
                if (day == null)
                {
                    columnSeriesTurnover.Items.Add(new ColumnItem(0));
                    lineSeriesBasket.Points.Add(new DataPoint(i, prevDay.Basket));
                }
                else
                {
                    columnSeriesTurnover.Items.Add(new ColumnItem(day.TotalTurnover));
                    lineSeriesBasket.Points.Add(new DataPoint(i, day.Basket));
                }
                if (day != null) prevDay = day;
            }

            plotModel.Series.Add(lineSeriesBasket);
            plotModel.Series.Add(columnSeriesTurnover);
        }
    }
}
