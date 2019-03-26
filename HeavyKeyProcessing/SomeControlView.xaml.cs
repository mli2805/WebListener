using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HeavyKeyProcessing
{
    /// <summary>
    /// Interaction logic for SomeControlView.xaml
    /// </summary>
    public partial class SomeControlView
    {
        public SomeControlView()
        {
            InitializeComponent();
        }

        private void TextBox1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Task.Factory.StartNew(HeavyCalculationAnotherThread);
        }

        private void HeavyCalculationAnotherThread()
        {
            Thread.Sleep(300);
            Application.Current.Dispatcher.Invoke(() => TextBlock1.Text = "result" + TextBox1.Text); // sync, GUI thread
        }

    }
}
