using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BanksListener
{
    public class BelgazMobi : IRatesLineExtractor
    {
        private DateTime _startDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private const string Url = "https://mobapp-frontend.bgpb.by/telecard-clientapi/rest/v4/andorid_5.12.1/currentRates?branch=561&type_rate=PNN&type_rate=SNN&type_rate=ENN&base_crnc=*&all_bounds=true";

        public async Task<KomBankRatesLine> GetRatesLineAsync()
        {
            var page = await ((HttpWebRequest)WebRequest.Create(Url))
                          .InitializeForKombanks()
                          .GetDataAsync();

            if (string.IsNullOrEmpty(page)) return null;
            var rates = (BelgazOneRate[])JsonConvert.DeserializeObject(page, typeof(BelgazOneRate[]));
            if (rates == null) return null;

            var result = new KomBankRatesLine() { Bank = KomBankE.Bgpb.ToString().ToUpper(), };
            DateTime date = _startDate.AddMilliseconds(rates[0].D).ToLocalTime();
            result.StartedFrom = date;
            result.LastCheck = DateTime.Now;

            result.EurA = rates.Where(r => r.T == "PNN" && r.C == "EUR").Max(l => l.V);
            result.EurB = rates.Where(r => r.T == "SNN" && r.C == "EUR").Min(l => l.V);

            result.UsdA = rates.Where(r => r.T == "PNN" && r.C == "USD").Max(l => l.V);
            result.UsdB = rates.Where(r => r.T == "SNN" && r.C == "USD").Min(l => l.V);

            result.RubA = rates.Where(r => r.T == "PNN" && r.C == "RUB").Max(l => l.V);
            result.RubB = rates.Where(r => r.T == "SNN" && r.C == "RUB").Min(l => l.V);

            result.EurUsdA = rates.First(r => r.T == "ENN" && r.C == "EUR" && r.Bc == "USD").V;
            result.EurUsdB = rates.First(r => r.T == "ENN" && r.C == "USD" && r.Bc == "EUR").V;

            result.RubUsdB = rates.First(r => r.T == "ENN" && r.C == "RUB" && r.Bc == "USD").V;
            result.RubUsdA = rates.First(r => r.T == "ENN" && r.C == "USD" && r.Bc == "RUB").V;

            result.RubEurB = rates.First(r => r.T == "ENN" && r.C == "RUB" && r.Bc == "EUR").V;
            result.RubEurA = rates.First(r => r.T == "ENN" && r.C == "EUR" && r.Bc == "RUB").V;

            return result;
        }
    }
}
