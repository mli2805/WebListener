using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AutoMapper;
using BalisStandard;
using Caliburn.Micro;
using Newtonsoft.Json;
using UtilsLib;

namespace BalisWpf
{
    public class KomBankViewModel
    {
        private static readonly IMapper Mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();

        private readonly string _baliApiUrl;
        public KomBankE KomBank;
        private readonly IMyLog _logFile;
        private readonly IWindowManager _windowManager;
        public string BankTitle => KomBank.GetAbbreviation();


        public ObservableCollection<KomBankRateVm> Rows { get; set; } = new ObservableCollection<KomBankRateVm>();

        public KomBankViewModel(IniFile iniFile, KomBankE komBank, IMyLog logFile, IWindowManager windowManager)
        {
            KomBank = komBank;
            _logFile = logFile;
            _windowManager = windowManager;
            _baliApiUrl = iniFile.Read(IniSection.General, IniKey.BaliApiUrl, "localhost:11082");
        }

        public void ShowForm()
        {
            var vm = new KomBankTnCViewModel();
            _windowManager.ShowWindow(vm);
        }

        public async Task StartPolling()
        {
            while (true)
            {
                await Task.Delay(3000);
                await GetOneLast();
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private async Task GetOneLast()
        {
            var webApiUrl = $@"http://{_baliApiUrl}/bali/get-one-last/" + KomBank.ToString().ToUpper();

            try
            {
                var response = await ((HttpWebRequest)WebRequest.Create(webApiUrl)).GetDataAsync();

                if (string.IsNullOrEmpty(response))
                {
                    var lastLine = Rows.Last();
                    lastLine.SetIfExpired();
                    return;
                }

                var oneLine = JsonConvert.DeserializeObject<KomBankRatesLine>(response);

                var newLine = Mapper.Map<KomBankRateVm>(oneLine);
                var last = Rows.FirstOrDefault(r => r.Id == newLine.Id);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (last == null)
                    {
                        newLine.State = "Fresh";
                        if (Rows.Any())
                            Rows.Last().State = "";
                        Rows.Add(newLine);
                        var notify = new Changes
                        {
                            MessageBlock = { Text = newLine.Bank + " " + newLine.StartedFrom }
                        };
                        notify.Show();
                    }
                    else
                    {
                        last.SetIfExpired();
                        last.LastCheck = newLine.LastCheck;
                    }
                });

            }
            catch (Exception e)
            {
                _logFile.AppendLine(e.Message);
            }
        }

        public async Task<KomBankViewModel> GetSomeLast()
        {
            var webApiUrl = $@"http://{_baliApiUrl}/bali/get-some-last/" + KomBank.ToString().ToUpper();

            try
            {
                var response = await ((HttpWebRequest)WebRequest.Create(webApiUrl)).GetDataAsync();
                var lastLines = JsonConvert.DeserializeObject<IEnumerable<KomBankRatesLine>>(response);

                foreach (var line in lastLines.Reverse())
                {
                    var vm = Mapper.Map<KomBankRateVm>(line);
                    Application.Current.Dispatcher.Invoke(() => { Rows.Add(vm); });
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
