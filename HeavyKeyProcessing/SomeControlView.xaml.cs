using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
//            Thread.Sleep(300);
//            TextBlock1.Text = "result" + TextBox1.Text;

            Task.Factory.StartNew(HeavyCalculationAnotherThread);
        }

        private void HeavyCalculationAnotherThread()
        {
            Thread.Sleep(300);
            if (Application.Current.Dispatcher != null)
                Application.Current.Dispatcher.Invoke(() =>
                    TextBlock1.Text = "result" + TextBox1.Text); // sync, GUI thread
        }

    }
}
