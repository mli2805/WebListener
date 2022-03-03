using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace BalisWpf
{
    public class MonthlyChartViewModel : Screen
    {
        private ObservableCollection<BelStockArchiveLine> Rates { get; set; } = new ObservableCollection<BelStockArchiveLine>();

        public PlotModel MyPlotModel { get; set; } = new PlotModel();
        public PlotModel MyPlotModel2 { get; set; } = new PlotModel();
        public void Initialize(ObservableCollection<BelStockArchiveLine> rows)
        {
            Rates = rows;
            // var start = new DateTime(2020, 9, 1);

            OneMonth(2020, 9, MyPlotModel, OxyColor.FromArgb(255, 0, 0, 255));
            OneMonth(2020, 10, MyPlotModel, OxyColor.FromArgb(255, 0, 255, 0));
            OneMonth(2020, 11, MyPlotModel, OxyColor.FromArgb(255, 255, 0, 0));
            OneMonth(2020, 12, MyPlotModel2, OxyColor.FromArgb(255, 0, 0, 255));
            OneMonth(2021, 1, MyPlotModel2, OxyColor.FromArgb(255, 0, 255, 0));
            OneMonth(2021, 2, MyPlotModel2, OxyColor.FromArgb(255, 255, 0, 0));

            MyPlotModel.Axes.Add(new LinearAxis() { Key = "basketAxis", Position = AxisPosition.Left });
            MyPlotModel.Axes.Add(new LinearAxis() { Key = "turnoverAxis", Position = AxisPosition.Right });

            MyPlotModel2.Axes.Add(new LinearAxis() { Key = "basketAxis", Position = AxisPosition.Left });
            MyPlotModel2.Axes.Add(new LinearAxis() { Key = "turnoverAxis", Position = AxisPosition.Right });
        }


        private void OneMonth(int year, int month, PlotModel plotModel, OxyColor color)
        {
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
