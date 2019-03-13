using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Extractors;

namespace WebListenerRx
{
    public static class HistoryLoader
    {
        public static void LoadKombanksHistory(MainVm mainVm)
        {
            Task.Factory.StartNew(() => LoadOneBankHistory(KomBank2.Bib, mainVm));
            Task.Factory.StartNew(() => LoadOneBankHistory(KomBank2.Prior, mainVm));
            Task.Factory.StartNew(() => LoadOneBankHistory(KomBank2.Mmb, mainVm));
            Task.Factory.StartNew(() => LoadOneBankHistory(KomBank2.Bps, mainVm));
            Task.Factory.StartNew(() => LoadOneBankHistory(KomBank2.Bgpb, mainVm));
        }

        private static void LoadOneBankHistory(KomBank2 komBank2, MainVm mainVm)
        {
            var temp = LoadLines(komBank2);
            Application.Current.Dispatcher.Invoke(() =>
            {
                mainVm.AssignModel(komBank2, new OneBankViewModel
                {
                    BankTitle = komBank2.GetAbbreviation(),
                    Rows = temp
                });
               });
        }

        private static ObservableCollection<KomBankRates> LoadLines(KomBank2 komBank2)
        {
            var result = new ObservableCollection<KomBankRates>();
            var content = File.ReadAllLines(komBank2.GetFilename());
            foreach (var line in content)
            {
                result.Add(new KomBankRates(line));
            }
            return result;
        }
    }
}