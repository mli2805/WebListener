using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;

namespace BalisStandard
{
    public class Banki24ArchiveManager
    {
        private readonly IMyLog _logFile;
        private readonly string _dbPath;

        public Banki24ArchiveManager(IniFile iniFile, IMyLog logFile)
        {
            _logFile = logFile;
            _dbPath = iniFile.Read(IniSection.Sqlite, IniKey.DbPath, "");
        }

        public async void StartThread()
        {
            await Task.Factory.StartNew(Poll);
        }

        private async Task Poll()
        {
            _logFile.AppendLine("Banki24 archive extractor started");
            var date = new DateTime(1, 1, 1);
            while (true)
            {
                if (date.Date < DateTime.Today.Date || (DateTime.Now.Hour >= 10 && DateTime.Now.Hour <= 13))
                    date = await UpdateDatabase();
                else
                    _logFile.AppendLine("Banki24: it is not a time to check archive.");
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
                {
                    await PersistRangeOfLines(newLines);
                    _logFile.AppendLine($"Banki24 archive for {date.Date:d} extracted");
                }
                else
                    _logFile.AppendLine($"Banki24 archive has no news for {date.Date:d}.");
                date = date.AddDays(1);
            }

            return date;
        }

        private async Task PersistRangeOfLines(List<BelStockArchiveOneCurrency> newLines)
        {
            await using BanksListenerContext db = new BanksListenerContext(_dbPath);
            db.Banki24Archive.AddRange(newLines);
            var result = await db.SaveChangesAsync();
            Console.WriteLine("PersistRangeOfLines " + result);
        }

        private async Task<List<BelStockArchiveOneCurrency>> GetArchiveFromDate(DateTime date)
        {
            var newLines = new List<BelStockArchiveOneCurrency>();
            var usdLine = await new Banki24ArchiveExtractor().GetOneCurrencyDayAsync(date, Currency.Usd);
            if (usdLine != null)
                newLines.Add(usdLine);
            var eurLine = await new Banki24ArchiveExtractor().GetOneCurrencyDayAsync(date, Currency.Eur);
            if (eurLine != null)
                newLines.Add(eurLine);
            var rubLine = await new Banki24ArchiveExtractor().GetOneCurrencyDayAsync(date, Currency.Rub);
            if (rubLine != null)
                newLines.Add(rubLine);
            var cnyLine = await new Banki24ArchiveExtractor().GetOneCurrencyDayAsync(date, Currency.Cny);
            if (cnyLine != null)
                newLines.Add(cnyLine);
            return newLines;
        }

        private async Task<DateTime> GetDateToStartFrom()
        {
            using (BanksListenerContext db = new BanksListenerContext(_dbPath))
            {
                var lastDateLine = db.Banki24Archive.OrderBy(d => d.Date).LastOrDefault();
                if (lastDateLine == null)
                    return new DateTime(2015, 6, 1);

                db.Banki24Archive.RemoveRange(db.Banki24Archive.Where(l => l.Date.Date == lastDateLine.Date.Date));
                await db.SaveChangesAsync();
                return lastDateLine.Date.Date;
            }
        }

    }
}