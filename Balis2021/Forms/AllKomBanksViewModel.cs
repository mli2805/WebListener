using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using BalisStandard;
using Caliburn.Micro;
using Newtonsoft.Json;
using UtilsLib;

namespace Balis2021
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<KomBankRatesLine, KomBankRateVm>();
        }
    }
    
    public class AllKomBanksViewModel : PropertyChangedBase
    {
        private static readonly IMapper Mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();

        private readonly string _baliApiUrl; 
        private readonly IMyLog _logFile;
        private readonly IWindowManager _windowManager;
        public ObservableCollection<KomBankRateVm> Rows { get; set; } = new ObservableCollection<KomBankRateVm>();

        public AllKomBanksViewModel(IniFile iniFile, IMyLog logFile, IWindowManager windowManager)
        {
            _logFile = logFile;
            _windowManager = windowManager;
            _baliApiUrl = iniFile.Read(IniSection.General, IniKey.BaliApiUrl, "localhost:11082");
        }

        public async void Start()
        {
            foreach (var komBank in (KomBankE[])Enum.GetValues(typeof(KomBankE)))
            {
                await GetOneLast(komBank);
            }
        }

        private async Task GetOneLast(KomBankE komBank)
        {
            var webApiUrl = $@"http://{_baliApiUrl}/bali/get-one-last/" + komBank.ToString().ToUpper();

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

                        // _changesViewModel.AddNewLine(newLine);
                        // if (!_changesViewModel.IsOpen)
                        //     _windowManager.ShowWindow(_changesViewModel);

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

    }
}
