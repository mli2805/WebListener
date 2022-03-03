using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UtilsLib;

namespace BalisStandard
{
    public class KomBanksPoller
    {
        private readonly IMyLog _logFile;
        private readonly string _dbPath;

        public KomBanksPoller(IniFile iniFile, IMyLog logFile)
        {
            _logFile = logFile;
            // var loc = System.Reflection.Assembly.GetExecutingAssembly().Location;
            // var dbDir = loc.GetParentFolder().GetParentFolder();
            // iniFile.Write(IniSection.Sqlite, IniKey.DbPath, Path.Combine(dbDir, "bali.db"));
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
            await Task.Factory.StartNew(() => Poll(new AlfaExtractor()));
            await Task.Factory.StartNew(() => Poll(new MtbExtractor()));
            await Task.Factory.StartNew(() => Poll(new BnbExtractor()));
        }

        private async void Poll(IRatesLineExtractor ratesLineExtractor)
        {
            var tid = Thread.CurrentThread.ManagedThreadId;
            _logFile.AppendLine($"{ratesLineExtractor.BankTitle} extractor started in thread {tid}");
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
                await Task.Delay(ratesLineExtractor.BankTitle == "БНБ" ? 60000 : 15000);

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
                    if (rate.Bank == "BIB" || rate.Bank == "BNB")
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