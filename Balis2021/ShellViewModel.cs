using Caliburn.Micro;

namespace Balis2021 {
    public class ShellViewModel : Screen, IShell
    {
        public AllKomBanksViewModel AllKomBanksViewModel { get; }

        public ShellViewModel(AllKomBanksViewModel allKomBanksViewModel)
        {
            AllKomBanksViewModel = allKomBanksViewModel;
            allKomBanksViewModel.Start();
        }

        protected override void OnViewLoaded(object view)
        {
            DisplayName = "Balis 2021";
        }
    }
}