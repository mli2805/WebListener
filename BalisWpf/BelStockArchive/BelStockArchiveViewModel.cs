using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using BalisStandard;
using Caliburn.Micro;
using UtilsLib;

namespace BalisWpf
{
    public class BelStockArchiveViewModel : Screen
    {
        private readonly string _dbPath;
        private int _mode;

        private List<BelStockArchiveOneCurrencyDay> data;
        private List<BelStockArchiveLine> plainDailyData;
        private List<BelStockArchiveLine> monthData;
        private List<BelStockArchiveLine> dayOfMonthData;
        public ObservableCollection<BelStockArchiveLine> Rows { get; set; } = new ObservableCollection<BelStockArchiveLine>();

        public BelStockArchiveViewModel(ILifetimeScope container)
        {
            var iniFile = container.Resolve<IniFile>();
            _dbPath = iniFile.Read(IniSection.Sqlite, IniKey.DbPath, "");
        }

        public async Task Initialize()
        {
            await using BanksListenerContext db = new BanksListenerContext(_dbPath);
            data = db.BelStockArchive.ToList();
            plainDailyData = data
                .GroupBy(d => d.Date)
                .Select(ToBelStockArchiveDate)
                .ToList();

            InitializeMainTable();

            monthData = plainDailyData
                .GroupBy(d => $"{d.Date:MMM yyyy}")
                .Select(ToBelStockArchiveMonth)
                .ToList();

            dayOfMonthData = plainDailyData
                .GroupBy(d => $"{d.Date:dd}-е")
                .Select(ToBelStockArchiveDayOfMonth)
                .OrderBy(l => l.Timestamp)
                .ToList();
        }

        private void InitializeMainTable()
        {
            Rows.Clear();
            plainDailyData.Do(Rows.Add);
        }

        private void InitializeMonthTable()
        {
            Rows.Clear();
            monthData.Do(Rows.Add);
        }

        private void InitializeDayOfMonthTable()
        {
            Rows.Clear();
            dayOfMonthData.Do(Rows.Add);
        }

        private static BelStockArchiveLine ToBelStockArchiveMonth(IGrouping<string, BelStockArchiveLine> days)
        {
            var belStockMonth = new BelStockArchiveLine() { Timestamp = days.Key };
            belStockMonth.UsdTurnover = days.Sum(d => d.UsdTurnover);
            var usdTurnoverInByn = days.Sum(d => d.UsdRate * d.UsdTurnover);
            belStockMonth.UsdRate = usdTurnoverInByn / belStockMonth.UsdTurnover;

            belStockMonth.EuroTurnover = days.Sum(d => d.EuroTurnover);
            var euroTurnoverInByn = days.Sum(d => d.EuroRate * d.EuroTurnover);
            belStockMonth.EuroRate = euroTurnoverInByn / belStockMonth.EuroTurnover;

            belStockMonth.RubTurnover = days.Sum(d => d.RubTurnover);
            var rubTurnoverInByn = days.Sum(d => d.RubRate * d.RubTurnover);
            belStockMonth.RubRate = rubTurnoverInByn / belStockMonth.RubTurnover;

            return belStockMonth;
        }

        private static BelStockArchiveLine ToBelStockArchiveDayOfMonth(IGrouping<string, BelStockArchiveLine> days)
        {
            var belStockMonth = new BelStockArchiveLine() { Timestamp = days.Key };
            belStockMonth.UsdTurnover = days.Sum(d => d.UsdTurnover) / days.Count();
            belStockMonth.EuroTurnover = days.Sum(d => d.EuroTurnover) / days.Count();
            belStockMonth.RubTurnover = days.Sum(d => d.RubTurnover) / days.Count();
            return belStockMonth;
        }

        private static BelStockArchiveLine ToBelStockArchiveDate(IGrouping<DateTime, BelStockArchiveOneCurrencyDay> day)
        {
            var belStockDay = new BelStockArchiveLine() { Date = day.Key, Timestamp = $"{day.Key:dd.MM.yyyy}" };

            var usd = day.FirstOrDefault(l => l.Currency == Currency.Usd);
            if (usd == null) return belStockDay;
            belStockDay.UsdTurnover = usd.TurnoverInCurrency;
            belStockDay.UsdRate = usd.TurnoverInByn / usd.TurnoverInCurrency;

            var euro = day.FirstOrDefault(l => l.Currency == Currency.Eur);
            belStockDay.EuroTurnover = euro?.TurnoverInByn / belStockDay.UsdRate ?? 0;
            belStockDay.EuroRate = euro?.TurnoverInByn / euro?.TurnoverInCurrency ?? 0;

            var rub = day.FirstOrDefault(l => l.Currency == Currency.Rub);
            belStockDay.RubTurnover = rub?.TurnoverInByn / belStockDay.UsdRate ?? 0;
            belStockDay.RubRate = rub?.TurnoverInByn / rub?.TurnoverInCurrency * 100 ?? 0;

            return belStockDay;
        }

        public void Toggle()
        {
            switch (_mode)
            {
                case 0:
                    InitializeMonthTable();
                    break;
                case 1:
                    InitializeDayOfMonthTable();
                    break;
                case 2:
                    InitializeMainTable();
                    break;
            }

            _mode++;
            if (_mode == 3) _mode = 0;
         }

        public void SaveAs()
        {
            var gsh = new GoogleSheetsHelper(@"..\ini\MyTutorialGsheet-d6d0997cf1ec.json",
                "1U72wGk-LojflkxPAWH-rDv2FGc8iIvPiNRGTy7CTB3I/");
            var row1 = new GoogleSheetRow();
            var row2 = new GoogleSheetRow();
 
            var cell1 = new GoogleSheetCell() { CellValue = "Header 1", IsBold = true, BackgroundColor = Color.DarkGoldenrod};
            var cell2 = new GoogleSheetCell() { CellValue = "Header 2", BackgroundColor = Color.Cyan };
 
            var cell3 = new GoogleSheetCell() { CellValue = "Value 1"};
            var cell4 = new GoogleSheetCell() { CellValue = "Value 2"};
 
            row1.Cells.AddRange(new List<GoogleSheetCell>() {cell1, cell2});
            row2.Cells.AddRange(new List<GoogleSheetCell>() { cell3, cell4 });
 
            var rows = new List<GoogleSheetRow>() { row1, row2 };
 
            gsh.AddCells(new GoogleSheetParameters() {SheetName="Sheet1", RangeColumnStart = 1, RangeRowStart = 1 }, rows);
        }
    }
}
