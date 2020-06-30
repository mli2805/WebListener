using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using UtilsLib;

namespace BalisStandard
{
    public class Banki24ArchiveManager
    {
        private readonly IMyLog _logFile;
        private readonly string _dbPath;

        public Banki24ArchiveManager(ILifetimeScope container)
        {
            _logFile = container.Resolve<IMyLog>();
            var iniFile = container.Resolve<IniFile>();
            _dbPath = iniFile.Read(IniSection.Sqlite, IniKey.DbPath, "");
        }

        public async void StartThread()
        {
            await Task.Factory.StartNew(Poll);
        }

        private async void Poll()
        {
            _logFile.AppendLine("Banki24 archive extractor started");
            var date = new DateTime(1, 1, 1);
            while (true)
            {
                if (date.Date < DateTime.Today.Date || (DateTime.Now.Hour >= 10 && DateTime.Now.Hour <= 13))
                    date = await UpdateDatabase();
                await Task.Delay(15 * 60 * 1000);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private async Task<DateTime> UpdateDatabase()
        {
            var date = await GetDateToStartFrom();
            while (date <= DateTime.Today.Date)
            {
                var newLines = await GetArchiveFromDate(date);
                if (newLines.Any())
                    await PersistRangeOfLines(newLines);
                _logFile.AppendLine($"Banki24 archive for {date.Date:d} extracted");
                date = date.AddDays(1);
            }

            return date;
        }

        private async Task PersistRangeOfLines(List<BelStockArchiveOneCurrencyDay> newLines)
        {
            await using BanksListenerContext db = new BanksListenerContext(_dbPath);
            db.BelStockArchive.AddRange(newLines);
            await db.SaveChangesAsync();
        }

        private async Task<List<BelStockArchiveOneCurrencyDay>> GetArchiveFromDate(DateTime date)
        {
            var newLines = new List<BelStockArchiveOneCurrencyDay>();
            var usdLine = await new Banki24ArchiveExtractor().GetOneCurrencyDayAsync(date, Currency.Usd);
            if (usdLine != null)
                newLines.Add(usdLine);
            var eurLine = await new Banki24ArchiveExtractor().GetOneCurrencyDayAsync(date, Currency.Eur);
            if (eurLine != null)
                newLines.Add(eurLine);
            var rubLine = await new Banki24ArchiveExtractor().GetOneCurrencyDayAsync(date, Currency.Rub);
            if (rubLine != null)
                newLines.Add(rubLine);
            return newLines;
        }

        private async Task<DateTime> GetDateToStartFrom()
        {
            using (BanksListenerContext db = new BanksListenerContext(_dbPath))
            {
                var lastDateLine = db.BelStockArchive.OrderBy(d => d.Date).LastOrDefault();
                if (lastDateLine == null)
                    return new DateTime(2015, 6, 1);

                db.BelStockArchive.RemoveRange(db.BelStockArchive.Where(l => l.Date.Date == lastDateLine.Date.Date));
                await db.SaveChangesAsync();
                return lastDateLine.Date.Date;
            }
        }

    }
}