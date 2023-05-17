using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private int _mode;

        private List<BelStockArchiveOneCurrency> _data;
        private List<BelStockArchiveLine> _plainDailyData;
        private List<BelStockArchiveLine> _monthData;
        private List<BelStockArchiveLine> _dayOfMonthData;
        public ObservableCollection<BelStockArchiveLine> Rows { get; set; } = new ObservableCollection<BelStockArchiveLine>();

        public BelStockArchiveViewModel(IniFile iniFile, IWindowManager windowManager)
        {
            _iniFile = iniFile;
            _windowManager = windowManager;
        }

        protected override void OnViewLoaded(object view)
        {
            DisplayName = "Ежедневные обороты на бирже";
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
            InitializeMainTable();
        }

        private void InitializeMainTable()
        {
            DisplayName = "Дневные обороты на бирже";
            _plainDailyData ??= _data
                .GroupBy(d => d.Date)
                .Select(ToBelStockArchiveDate)
                .ToList();
            Rows.Clear();
            _plainDailyData.ForEach(Rows.Add);
        }

        private void InitializeMonthTable()
        {
            DisplayName = "Суммарные обороты за месяц, курс валют средние за месяц";
            _monthData ??= _plainDailyData
                .GroupBy(d => $"{d.Date:MMM yyyy}")
                .Select(ToBelStockArchiveMonth)
                .ToList();
            Rows.Clear();
            _monthData.ForEach(Rows.Add);
        }

        private void InitializeDayOfMonthTable()
        {
            DisplayName = "Средние обороты по дням месяца на бирже";
            _dayOfMonthData ??= _plainDailyData
                .GroupBy(d => $"{d.Date:dd}-е")
                .Select(ToBelStockArchiveDayOfMonth)
                .OrderBy(l => l.Timestamp)
                .ToList(); 
            Rows.Clear();
            _dayOfMonthData.ForEach(Rows.Add);
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
            if (belStockMonth.CnyTurnover > 0)
            {
                var cnyTurnoverInByn = days.Sum(d => d.CnyRate * d.CnyTurnover);
                belStockMonth.CnyRate = cnyTurnoverInByn / belStockMonth.CnyTurnover;
            }

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
            belStockDay.UsdTurnover = usd?.TurnoverInCurrency ?? 0;
            belStockDay.UsdRate = usd?.Average ?? 0;

            var euro = day.FirstOrDefault(l => l.Currency == Currency.Eur);
            belStockDay.EuroTurnover = euro?.TurnoverInUsd ?? 0;
            belStockDay.EuroRate = euro?.Average ?? 0;

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

      

        public void MonthlyChart()
        {
            var vm = new MonthlyChartViewModel();
            vm.Initialize(Rows);
            _windowManager.ShowWindow(vm);
        }
    }
}
