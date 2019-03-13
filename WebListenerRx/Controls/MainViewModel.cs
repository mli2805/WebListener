using Caliburn.Micro;

namespace WebListenerRx
{
    public class MainViewModel : PropertyChangedBase
    {
        public MainVm MainVm { get; set; } = new MainVm();

        public MainViewModel()
        {
            MainVm.PropertyChanged += MainVm_PropertyChanged;
        }

        private void MainVm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AllBanksLoaded")
                StartKomBanksPolling();
        }

        private void StartKomBanksPolling()
        {
        //    var l = MainVm.BgpbModel.Rows.Last().Clone();
  //          MainVm.BgpbModel.Rows.Add(l);
        }

        public void LoadHistory()
        {
            HistoryLoader.LoadKombanksHistory(MainVm);
        }


    }
}
