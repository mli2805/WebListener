using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UtilsLib;

namespace BalisStandard
{
    public class KomBanksPoller
    {
        private readonly ILifetimeScope _container;
        private readonly IMyLog _logFile;
        private readonly string _dbPath;
        private ForPollerSignalRClient _forPollerSignalRClient;

        public KomBanksPoller(ILifetimeScope container)
        {
            _container = container;
            _logFile = container.Resolve<IMyLog>();
            var iniFile = container.Resolve<IniFile>();
            _dbPath = iniFile.Read(IniSection.Sqlite, IniKey.DbPath, "");
        }

        public async void StartThreads()
        {
            await Task.Factory.StartNew(() => Poll(new BelgazMobi()));
            await Task.Factory.StartNew(() => Poll(new BibExtractor()));
            await Task.Factory.StartNew(() => Poll(new PriorExtractor()));
            await Task.Factory.StartNew(() => Poll(new DabrabytExtractor()));
            await Task.Factory.StartNew(() => Poll(new BelvebExtractor()));
            //            await Task.Factory.StartNew(() => Poll(new BpsExtractor()));
        }

        private async void Poll(IRatesLineExtractor ratesLineExtractor)
        {
            var tid = Thread.CurrentThread.ManagedThreadId;
            _logFile.AppendLine($"{ratesLineExtractor.BankTitle} extractor started in thread {tid}");
         //   _balisSignalRClient = _container.Resolve<BalisSignalRClient>();
            _forPollerSignalRClient = _container.Resolve<ForPollerSignalRClient>();
            _forPollerSignalRClient.Start();
            while (true)
            {
                KomBankRatesLine rate;
                while (true)
                {
                    rate = await ratesLineExtractor.GetRatesLineAsync();
                    if (rate != null) break;
                    await Task.Delay(2000);
                }

                var unused = await Persist(rate);
                await Task.Delay(15000);

            }
            // ReSharper disable once FunctionNeverReturns
        }

        private async Task<int> Persist(KomBankRatesLine rate)
        {
            using (BanksListenerContext db = new BanksListenerContext(_dbPath))
            {
                var tid = Thread.CurrentThread.ManagedThreadId;
                var last = await db.KomBankRates.Where(l => l.Bank == rate.Bank).OrderBy(c => c.LastCheck).LastOrDefaultAsync();
                if (last == null || last.IsDifferent(rate))
                {
                    if (rate.Bank == KomBankE.Bib.ToString().ToUpper())
                        rate.StartedFrom = DateTime.Now; // Bib page does not contain date from
                    db.KomBankRates.Add(rate);
                    _logFile.AppendLine($"Thread id {tid}: {rate.Bank} new rate, usd {rate.UsdA} - {rate.UsdB},  euro {rate.EurA} - {rate.EurB},  rub {rate.RubA} - {rate.RubB}");
                    _forPollerSignalRClient.NotifyAll("RateChanged", JsonConvert.SerializeObject(rate));
                }
                else
                {
                    last.LastCheck = DateTime.Now;
                    _logFile.AppendLine($"Thread id {tid}: Poller of {rate.Bank} - rate checked");
                    _forPollerSignalRClient.NotifyAll("TheSameRate", JsonConvert.SerializeObject(rate));
                }

                return await db.SaveChangesAsync();
            }
        }

    }
}