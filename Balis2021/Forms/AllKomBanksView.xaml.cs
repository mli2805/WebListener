using System.Windows.Controls;

namespace Balis2021
{
    /// <summary>
    /// Interaction logic for AllKomBanksView.xaml
    /// </summary>
    public partial class AllKomBanksView
    {
        public AllKomBanksView()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((DataGrid)sender).UnselectAllCells();
        }
    }
}
