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

        private readonly string _baliApiUrl;
        public KomBankE KomBank;
        private readonly IMyLog _logFile;
        public string BankTitle => KomBank.GetAbbreviation();

        public ObservableCollection<KomBankRateVm> Rows { get; set; } = new ObservableCollection<KomBankRateVm>();

        public KomBankViewModel(IniFile iniFile, KomBankE komBank, IMyLog logFile)
        {
            KomBank = komBank;
            _logFile = logFile;
            _baliApiUrl = iniFile.Read(IniSection.General, IniKey.BaliApiUrl, "localhost:11082");
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
                    lastLine.State = DateTime.Now - lastLine.LastCheck > TimeSpan.FromSeconds(45) ? "Expired" : "";
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
                        last.State = DateTime.Now - newLine.LastCheck > TimeSpan.FromSeconds(45) ? "Expired" : "";
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
