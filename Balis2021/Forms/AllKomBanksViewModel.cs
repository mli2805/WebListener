using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
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

        public ObservableCollection<KomBankRateVm> Rows { get; set; } = new ObservableCollection<KomBankRateVm>();
        public ICollectionView SortedRows { get; set; }

        public AllKomBanksViewModel(IniFile iniFile, IMyLog logFile)
        {
            _logFile = logFile;
            _baliApiUrl = iniFile.Read(IniSection.General, IniKey.BaliApiUrl, "localhost:11082");
        }

        public async void Start()
        {
            foreach (var komBank in (KomBankE[])Enum.GetValues(typeof(KomBankE)))
            {
                if (komBank == KomBankE.Bps) continue;
                await GetOneLast(komBank);
            }

            SortedRows = CollectionViewSource.GetDefaultView(Rows);
            SortedRows.SortDescriptions.Add(new SortDescription("UsdA", ListSortDirection.Descending));
        }

        public void ButtonChangeSort()
        {
            SortedRows.SortDescriptions.Clear();
            SortedRows.SortDescriptions.Add(new SortDescription("UsdB", ListSortDirection.Ascending));
        }

        private async Task GetOneLast(KomBankE komBank)
        {
            var webApiUrl = $@"http://{_baliApiUrl}/bali/get-one-last/" + komBank.ToString().ToUpper();

            try
            {
                var response = await ((HttpWebRequest)WebRequest.Create(webApiUrl)).GetDataAsync();
                Application.Current.Dispatcher.Invoke(() =>
                {
                   OneToGui(komBank, response);
                });

            }
            catch (Exception e)
            {
                _logFile.AppendLine(e.Message);
            }
        }

        public async Task GetSomeLast(KomBankE komBank)
        {
            var webApiUrl = $@"http://{_baliApiUrl}/bali/get-some-last/" + komBank.ToString().ToUpper();

            try
            {
                var response = await ((HttpWebRequest)WebRequest.Create(webApiUrl)).GetDataAsync();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ManyToGui(response);
                });

            }
            catch (Exception e)
            {
                _logFile.AppendLine(e.Message);
            }
        }

        private void ManyToGui(string response)
        {
            
            if (string.IsNullOrEmpty(response))
            {
                return;
            }

            var lastLines = JsonConvert.DeserializeObject<IEnumerable<KomBankRatesLine>>(response);

            foreach (var line in lastLines.Reverse())
            {
                var vm = Mapper.Map<KomBankRateVm>(line);
                Application.Current.Dispatcher.Invoke(() => { Rows.Add(vm); });
            }
        }


        private void OneToGui(KomBankE komBank, string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                var lastLine = Rows.LastOrDefault(r=>r.Bank == komBank.GetAbbreviation());
                lastLine?.SetIfExpired();
                return;
            }

            var oneLine = JsonConvert.DeserializeObject<KomBankRatesLine>(response);
            var newLine = Mapper.Map<KomBankRateVm>(oneLine);
            
            var last = Rows.FirstOrDefault(r => r.Id == newLine.Id);
            if (last == null)
            {
                newLine.State = "Fresh";
                if (Rows.Any())
                    Rows.Last().State = "";
                Rows.Add(newLine);

            }
            else
            {
                last.SetIfExpired();
                last.LastCheck = newLine.LastCheck;
            }
        }

    }

    

}
