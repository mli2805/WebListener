using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autofac;
using BalisStandard;
using Caliburn.Micro;
using UtilsLib;

namespace BalisWpf
{
    public static class EnumerableExt
    {
        public static void Do<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable)
            {
                action(element);
            }
        }
    }
    public class BelStockArchiveViewModel : Screen
    {
        private readonly string _dbPath;

        private List<BelStockArchiveOneCurrencyDay> data;
        public ObservableCollection<BelStockArchiveDay> Rows { get; set; } = new ObservableCollection<BelStockArchiveDay>();

        public BelStockArchiveViewModel(ILifetimeScope container)
        {
            var iniFile = container.Resolve<IniFile>();
            _dbPath = iniFile.Read(IniSection.Sqlite, IniKey.DbPath, "");
        }

        public async void Initialize()
        {
            await using BanksListenerContext db = new BanksListenerContext(_dbPath);
            data = db.BelStockArchive.ToList();
            Rows.Clear();
            data
                .GroupBy(d => d.Date)
                .Select(ToBelStockArchiveDay)
                .Do(Rows.Add);
        }

        private static BelStockArchiveDay ToBelStockArchiveDay(IGrouping<DateTime, BelStockArchiveOneCurrencyDay> day)
        {
            var belStockDay = new BelStockArchiveDay() {Date = day.Key.Date,};

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
    }
}
