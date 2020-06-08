using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using BalisStandard;
using Newtonsoft.Json;

namespace BalisWpf
{
    public class KomBankViewModel
    {
        private static readonly IMapper Mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();

        public KomBankE KomBank;
        public ObservableCollection<KomBankRateVm> Rows { get; set; } = new ObservableCollection<KomBankRateVm>();

        public KomBankViewModel(KomBankE komBank)
        {
            KomBank = komBank;
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
