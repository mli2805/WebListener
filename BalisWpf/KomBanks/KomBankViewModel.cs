using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using BalisStandard;
using Newtonsoft.Json;
using UtilsLib;

namespace BalisWpf
{
    public class KomBankViewModel
    {
        private static readonly IMapper Mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();

        public KomBankE KomBank;
        private readonly IMyLog _logFile;
        private readonly BalisSignalRClient _balisSignalRClient;
        public string BankTitle => KomBank.GetAbbreviation();
        public ObservableCollection<KomBankRateVm> Rows { get; set; } = new ObservableCollection<KomBankRateVm>();

        public KomBankViewModel(KomBankE komBank, IMyLog logFile, BalisSignalRClient balisSignalRClient)
        {
            KomBank = komBank;
            _logFile = logFile;
            _balisSignalRClient = balisSignalRClient;
            balisSignalRClient.PropertyChanged += BalisSignalRClient_PropertyChanged;
        }

        private void BalisSignalRClient_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "EventProperty") return;
            var data = JsonConvert.DeserializeObject<KomBankRatesLine>(_balisSignalRClient.EventProperty.Item2);
            if (data.Bank != KomBank.ToString().ToUpper()) return;
            if (_balisSignalRClient.EventProperty.Item1 == "RateChanged")
            {
                _logFile.AppendLine($"Rate changed in: {data.Bank} at {data.LastCheck}");
                var vm = Mapper.Map<KomBankRateVm>(data);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Rows.Add(vm);
                    MessageBox.Show($"{data.Bank} rates changed at {data.StartedFrom}");
                });
            }
            else
            {
                _logFile.AppendLine($"The same rate for: {data.Bank} at {data.LastCheck}");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var last = Rows.Last();
                    last.LastCheck = data.LastCheck;
                });
            }
        }

        public async Task<KomBankViewModel> GetLastFive()
        {
            var webApiUrl = @"http://localhost:8012/bali/get-last-five/" + KomBank.ToString().ToUpper();

            var response = await ((HttpWebRequest)WebRequest.Create(webApiUrl)).GetDataAsync();
            var lastFive = JsonConvert.DeserializeObject<IEnumerable<KomBankRatesLine>>(response);

            foreach (var line in lastFive.Reverse())
            {
                var vm = Mapper.Map<KomBankRateVm>(line);
                Rows.Add(vm);
            }

            return this;
        }

    }
}
