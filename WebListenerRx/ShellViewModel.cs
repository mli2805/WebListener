using System.Threading.Tasks;
using Caliburn.Micro;

namespace WebListenerRx
{
    public class ShellViewModel : Screen, IShell
    {
        public MainViewModel MainViewModel { get; set; } = new MainViewModel();
        public ShellViewModel()
        {
        }

        protected override void OnViewLoaded(object view)
        {
           Task.Factory.StartNew(()=>MainViewModel.LoadHistory());
        }

     
    }
}