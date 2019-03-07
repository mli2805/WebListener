using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Extractors;
using WebListener.Properties;

namespace WebListener
{
    /// <summary>
    /// Interaction logic for BelStockArchieveView.xaml
    /// </summary>
    public partial class BelStockArchieveView : INotifyPropertyChanged
    {
        private ObservableCollection<BelStockArchiveDay> _rows;
        public ObservableCollection<BelStockArchiveDay> Rows
        {
            get { return _rows; }
            set
            {
                if (Equals(value, _rows)) return;
                _rows = value;
                OnPropertyChanged();
            }
        }

        private readonly BelStockFileOperator _belStockFileOperator = new BelStockFileOperator();

        public BelStockArchieveView()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = "Bel Stock Archieve";
            Rows = new ObservableCollection<BelStockArchiveDay>(_belStockFileOperator.LoadBelStockArchieve());
            await DownloadNewLinesFromBanki24(GetLastPlusOneDay());
        }

        private DateTime GetLastPlusOneDay()
        {
            if (Rows.LastOrDefault() != null) Rows.Remove(Rows.Last()); // последняя линия могла быть получена до окончания торгов
            return (Rows.LastOrDefault() == null) ? new DateTime(2015, 6, 1) : Rows.Last().Date.AddDays(1);
        }
        private async Task DownloadNewLinesFromBanki24(DateTime startFrom)
        {
            var asyncExtractor = new Banki24ArchiveAsyncExtractor();
            var date = startFrom;
            while (date <= DateTime.Today.Date)
            {
                var result = await asyncExtractor.GetBelStockDayAsync(date);
                if (result != null) Rows.Add(result);
                date = date.AddDays(1);
            }
            _belStockFileOperator.SaveBelStockArchieve(Rows);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
