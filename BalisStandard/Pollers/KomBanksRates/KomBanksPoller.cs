using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UtilsLib;

namespace BalisStandard
{
    public class OnePoller
    {
        public IRatesLineExtractor KomBankExtractor { get; set; }
        public int PollingPeriod { get; set; }

        public OnePoller(IRatesLineExtractor komBankExtractor, int pollingPeriod)
        {
            KomBankExtractor = komBankExtractor;
            PollingPeriod = pollingPeriod;
        }
    }
    public class KomBanksPoller
    {
        private readonly IMyLog _logFile;
        private readonly string _dbPath;

        private readonly List<OnePoller> _pollers = new List<OnePoller>();

        public KomBanksPoller(IniFile iniFile, IMyLog logFile)
        {
            _logFile = logFile;
            // iniFile.Write(IniSection.Sqlite, IniKey.DbPath, Path.Combine(dbDir, "bali.db"));
            _dbPath = iniFile.Read(IniSection.Sqlite, IniKey.DbPath, "");

            _pollers.Add(new OnePoller(new AlfaExtractor(), iniFile.Read(IniSection.Extractors, IniKey.AlfaPeriod, 15)));
            _pollers.Add(new OnePoller(new BelgazMobi(), iniFile.Read(IniSection.Extractors, IniKey.BelgazPeriod, 15)));
            // _pollers.Add(new OnePoller(new BelvebExtractor(), iniFile.Read(IniSection.Extractors, IniKey.BelvebPeriod, 15)));
            _pollers.Add(new OnePoller(new BibExtractor(), iniFile.Read(IniSection.Extractors, IniKey.BibPeriod, 15)));
            _pollers.Add(new OnePoller(new BnbExtractor(), iniFile.Read(IniSection.Extractors, IniKey.BnbPeriod, 60)));
            _pollers.Add(new OnePoller(new BpsExtractor(), iniFile.Read(IniSection.Extractors, IniKey.BpsPeriod, 0)));
            _pollers.Add(new OnePoller(new DabrabytExtractor(), iniFile.Read(IniSection.Extractors, IniKey.DabrabytPeriod, 15)));
            _pollers.Add(new OnePoller(new MtbExtractor(), iniFile.Read(IniSection.Extractors, IniKey.MtbPeriod, 15)));
            _pollers.Add(new OnePoller(new PriorExtractor(), iniFile.Read(IniSection.Extractors, IniKey.PriorPeriod, 15)));
            _pollers.Add(new OnePoller(new VtbExtractor(), iniFile.Read(IniSection.Extractors, IniKey.VtbPeriod, 60)));
        }

        public async void StartThreads()
        {
            foreach (var onePoller in _pollers.Where(onePoller => onePoller.PollingPeriod != 0))
            {
                await Task.Factory.StartNew(() => Poll(onePoller));
            }
        }

        private async void Poll(OnePoller poller)
        {
            var tid = Thread.CurrentThread.ManagedThreadId;
            _logFile.AppendLine($"{poller.KomBankExtractor.BankTitle} extractor started in thread {tid}");
            while (true)
            {
                KomBankRatesLine rate;
                while (true)
                {
                    rate = await poller.KomBankExtractor.GetRatesLineAsync();
                    if (rate != null)
                        break;

                    _logFile.AppendLine($"Poller of {poller.KomBankExtractor.BankTitle} - failed to extract rates. Additional pause {poller.PollingPeriod} sec");
                    await Task.Delay(poller.PollingPeriod * 1000);
                }

                var unused = await Persist(rate);
                await Task.Delay(poller.PollingPeriod * 1000);

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
                    if (rate.Bank == "BIB" || rate.Bank == "BNB" || rate.Bank == "VTB")
                        rate.StartedFrom = DateTime.Now; // Bib page does not contain date from
                    if (rate.Bank == "BVEB" && rate.StartedFrom.Hour == 0 && DateTime.Now.Hour != 0)
                        rate.StartedFrom = DateTime.Now; // Bveb often returns 00:10 or 00:15 as start time
                    _logFile.AppendLine($"Thread id {tid}: {rate.Bank} new rate, usd {rate.UsdA} - {rate.UsdB},  euro {rate.EurA} - {rate.EurB},  rub {rate.RubA} - {rate.RubB}");

                    const double tolerance = 0.00001;
                    if (rate.Bank == "BNB" && Math.Abs(rate.UsdA - 2.619) < tolerance
                                    && Math.Abs(rate.UsdB - 2.625) < tolerance
                                    && Math.Abs(rate.EurA - 3.168) < tolerance
                                    && Math.Abs(rate.EurB - 3.175) < tolerance) return 0;

                    await db.KomBankRates.AddAsync(rate);
                }
                else
                {
                    last.LastCheck = DateTime.Now;
                    _logFile.AppendLine($"Thread id {tid}: Poller of {rate.Bank} - rate checked");
                }

                return await db.SaveChangesAsync();
            }
        }

    }
}