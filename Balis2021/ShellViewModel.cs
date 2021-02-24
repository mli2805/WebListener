using Caliburn.Micro;

namespace Balis2021 {
    public class ShellViewModel : Screen, IShell
    {
        private readonly IWindowManager _windowManager;

        public ShellViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        protected override void OnViewLoaded(object view)
        {
            DisplayName = "Balis 2021";
        }
    }
}