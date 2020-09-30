using System;
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

        private readonly IniFile _iniFile;
        public KomBankE KomBank;
        private readonly IMyLog _logFile;
        private readonly BalisSignalRClient _balisSignalRClient;
        public string BankTitle => KomBank.GetAbbreviation();
        public ObservableCollection<KomBankRateVm> Rows { get; set; } = new ObservableCollection<KomBankRateVm>();

        public KomBankViewModel(IniFile iniFile, KomBankE komBank, IMyLog logFile, BalisSignalRClient balisSignalRClient)
        {
            _iniFile = iniFile;
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
                    var notify = new Changes
                    {
                        MessageBlock = { Text = vm.Bank + " " + vm.StartedFrom }
                    };
                    notify.Show();   
                    //MessageBox.Show($"{data.Bank} rates changed at {data.StartedFrom}");
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

        public async Task<KomBankViewModel> GetSomeLast()
        {
            var baliApiUrl = _iniFile.Read(IniSection.General, IniKey.BaliApiUrl, "localhost:11081");
            var webApiUrl = $@"http://{baliApiUrl}/bali/get-some-last/" + KomBank.ToString().ToUpper();

            try
            {
                var response = await ((HttpWebRequest)WebRequest.Create(webApiUrl)).GetDataAsync();
                var lastFive = JsonConvert.DeserializeObject<IEnumerable<KomBankRatesLine>>(response);

                foreach (var line in lastFive.Reverse())
                {
                    var vm = Mapper.Map<KomBankRateVm>(line);
                    Rows.Add(vm);
                }
            }
            catch (Exception e)
            {
                _logFile.AppendLine(e.Message);
            }

            return this;
        }

    }
}
