using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BalisStandard;
using Caliburn.Micro;
using Newtonsoft.Json;
using UtilsLib;

namespace BalisWpf
{
    public class BelStockArchiveViewModel : Screen
    {
        private readonly IniFile _iniFile;
        private readonly IWindowManager _windowManager;
        private readonly MonthlyChartViewModel _monthlyChartViewModel;
        private int _mode;

        private List<BelStockArchiveOneCurrency> _data;
        private List<BelStockArchiveLine> _plainDailyData;
        private List<BelStockArchiveLine> _monthData;
        private List<BelStockArchiveLine> _dayOfMonthData;
        public ObservableCollection<BelStockArchiveLine> Rows { get; set; } = new ObservableCollection<BelStockArchiveLine>();

        public BelStockArchiveViewModel(IniFile iniFile, IWindowManager windowManager, MonthlyChartViewModel monthlyChartViewModel)
        {
            _iniFile = iniFile;
            _windowManager = windowManager;
            _monthlyChartViewModel = monthlyChartViewModel;
        }

        private async Task Fetch()
        {
            var baliApiUrl = _iniFile.Read(IniSection.General, IniKey.BaliApiUrl, "localhost:11082");
            int portionSize = _iniFile.Read(IniSection.General, IniKey.BelstockPortionSize, 100);
            _data = new List<BelStockArchiveOneCurrency>();
            try
            {
                int portionNumber = 0;
                while (true)
                {
                    var webApiUrl = $@"http://{baliApiUrl}/bali/get-banki24-archive/{portionNumber}";
                    var response = await ((HttpWebRequest)WebRequest.Create(webApiUrl)).GetDataAsync();
                    var portion = JsonConvert.DeserializeObject<List<BelStockArchiveOneCurrency>>(response);
                    _data.AddRange(portion);
                    if (portion.Count < portionSize) break;
                    portionNumber++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task Initialize()
        {
            await Fetch();
            _plainDailyData = _data
                .GroupBy(d => d.Date)
                .Select(ToBelStockArchiveDate)
                .ToList();

            InitializeMainTable();

            _monthData = _plainDailyData
                .GroupBy(d => $"{d.Date:MMM yyyy}")
                .Select(ToBelStockArchiveMonth)
                .ToList();

            _dayOfMonthData = _plainDailyData
                .GroupBy(d => $"{d.Date:dd}-е")
                .Select(ToBelStockArchiveDayOfMonth)
                .OrderBy(l => l.Timestamp)
                .ToList();
        }

        private void InitializeMainTable()
        {
            Rows.Clear();
            _plainDailyData.Do(Rows.Add);
        }

        private void InitializeMonthTable()
        {
            Rows.Clear();
            _monthData.Do(Rows.Add);
        }

        private void InitializeDayOfMonthTable()
        {
            Rows.Clear();
            _dayOfMonthData.Do(Rows.Add);
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

            belStockMonth.CnyTurnover = days.Sum(d => d.CnyTurnover);
            var cnyTurnoverInByn = days.Sum(d => d.CnyRate * d.CnyTurnover);
            belStockMonth.CnyRate = cnyTurnoverInByn / belStockMonth.CnyTurnover;

            return belStockMonth;
        }

        private static BelStockArchiveLine ToBelStockArchiveDayOfMonth(IGrouping<string, BelStockArchiveLine> days)
        {
            var belStockMonth = new BelStockArchiveLine() { Timestamp = days.Key };
            belStockMonth.UsdTurnover = days.Sum(d => d.UsdTurnover) / days.Count();
            belStockMonth.EuroTurnover = days.Sum(d => d.EuroTurnover) / days.Count();
            belStockMonth.RubTurnover = days.Sum(d => d.RubTurnover) / days.Count();
            belStockMonth.CnyTurnover = days.Sum(d => d.CnyTurnover) / days.Count();
            return belStockMonth;
        }

        private static BelStockArchiveLine ToBelStockArchiveDate(IGrouping<DateTime, BelStockArchiveOneCurrency> day)
        {
            var belStockDay = new BelStockArchiveLine() { Date = day.Key, Timestamp = $"{day.Key:dd.MM.yyyy}" };

            var usd = day.FirstOrDefault(l => l.Currency == Currency.Usd);
            if (usd == null) return belStockDay;
            belStockDay.UsdTurnover = usd.TurnoverInCurrency;
            belStockDay.UsdRate = usd.Average;

            var euro = day.FirstOrDefault(l => l.Currency == Currency.Eur);
            if (euro == null) return belStockDay;
            belStockDay.EuroTurnover = euro.TurnoverInUsd;
            belStockDay.EuroRate = euro.Average;

            var rub = day.FirstOrDefault(l => l.Currency == Currency.Rub);
            belStockDay.RubTurnover = rub?.TurnoverInUsd ?? 0;
            belStockDay.RubRate = rub?.Average ?? 0;

            var cny = day.FirstOrDefault(l => l.Currency == Currency.Cny);
            belStockDay.CnyTurnover = cny?.TurnoverInUsd ?? 0;
            belStockDay.CnyRate = cny?.Average ?? 0;

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

            var cell1 = new GoogleSheetCell() { CellValue = "Header 1", IsBold = true, BackgroundColor = Color.DarkGoldenrod };
            var cell2 = new GoogleSheetCell() { CellValue = "Header 2", BackgroundColor = Color.Cyan };

            var cell3 = new GoogleSheetCell() { CellValue = "Value 1" };
            var cell4 = new GoogleSheetCell() { CellValue = "Value 2" };

            row1.Cells.AddRange(new List<GoogleSheetCell>() { cell1, cell2 });
            row2.Cells.AddRange(new List<GoogleSheetCell>() { cell3, cell4 });

            var rows = new List<GoogleSheetRow>() { row1, row2 };

            gsh.AddCells(new GoogleSheetParameters() { SheetName = "Sheet1", RangeColumnStart = 1, RangeRowStart = 1 }, rows);
        }

        public void MonthlyChart()
        {
            _monthlyChartViewModel.Initialize(Rows);
            _windowManager.ShowWindow(_monthlyChartViewModel);
        }
    }
}
