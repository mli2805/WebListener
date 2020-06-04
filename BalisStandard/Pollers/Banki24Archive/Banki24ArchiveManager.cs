using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;

namespace BalisStandard
{
    public class Banki24ArchiveManager
    {
        private readonly IMyLog _logFile;
        private readonly string _dbPath;

        public Banki24ArchiveManager(IMyLog logFile, string dbPath)
        {
            _logFile = logFile;
            _dbPath = dbPath;
        }

        public void RunUpdatingInBackground()
        {
            var bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.DoWork += Bw_DoWork;
            bw.ProgressChanged += Bw_ProgressChanged;

            bw.RunWorkerAsync();
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            UpdateDatabase(worker);
        }

        // UI thread
        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var st = (string)e.UserState;
            _logFile.AppendLine(st);
        }

        private void UpdateDatabase(BackgroundWorker worker)
        {
            var date =  GetDateToStartFrom().Result;
            var newLines =  GetArchiveFromDate(date, worker).Result;
            PersistRangeOfLines(newLines).Wait();
        }

        private async Task<int> PersistRangeOfLines(IEnumerable<BelStockArchiveOneCurrencyDay> newLines)
        {
            using (BanksListenerContext db = new BanksListenerContext(_dbPath))
            {
                db.BelStockArchive.AddRange(newLines);
                return await db.SaveChangesAsync();
            }
        }

        private async Task<List<BelStockArchiveOneCurrencyDay>> GetArchiveFromDate(DateTime date, BackgroundWorker worker)
        {
            var newLines = new List<BelStockArchiveOneCurrencyDay>();
            while (date <= DateTime.Today.Date)
            {
                var usdLine = await new Banki24ArchiveExtractor().GetOneCurrencyDayAsync(date, Currency.Usd);
                if (usdLine != null)
                {
                    newLines.Add(usdLine);
                    var eurLine = await new Banki24ArchiveExtractor().GetOneCurrencyDayAsync(date, Currency.Eur);
                    if (eurLine != null)
                        newLines.Add(eurLine);
                    var rubLine = await new Banki24ArchiveExtractor().GetOneCurrencyDayAsync(date, Currency.Rub);
                    if (rubLine != null)
                        newLines.Add(rubLine);
                }

                date = date.AddDays(1);
                worker.ReportProgress(0, $"GetArchiveFromDate {date}");
            }
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