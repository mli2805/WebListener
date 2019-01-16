using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using WebListener.DomainModel.BelStock;

namespace WebListener.Views
{
    /// <summary>
    /// Interaction logic for BelStockChart.xaml
    /// </summary>
    public partial class BelStockChart
    {
        public PlotModel ChartModel { get; set; }
        public ColumnSeries VolumeSeries { get; set; }
        public BelStockChart()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void Initialize(ObservableCollection<BelStockArchiveDay> rows)
        {
            ChartModel = new PlotModel();
            VolumeSeries = new ColumnSeries() { Title = "Volume" };

            var labels = new List<string>();
            foreach (var day in rows)
            {
                labels.Add(day.Date.ToString("dd/MM/yyyy"));
                VolumeSeries.Items.Add(new ColumnItem(day.Usd.TurnoverInCurrency, 0));
                VolumeSeries.Items.Add(new ColumnItem(day.Eur.TurnoverInCurrency, 1));
            }

            ChartModel.Axes.Add(new CategoryAxis() { Labels = { "usd", "euro" } });
            ChartModel.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Dash });
        }
    }
}
