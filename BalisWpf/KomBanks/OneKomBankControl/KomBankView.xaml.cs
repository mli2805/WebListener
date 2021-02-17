using System.Windows.Controls;

namespace BalisWpf
{
    /// <summary>
    /// Interaction logic for KomBankView.xaml
    /// </summary>
    public partial class KomBankView
    {
        public KomBankView()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((DataGrid)sender).UnselectAllCells();
        }

    }
}
