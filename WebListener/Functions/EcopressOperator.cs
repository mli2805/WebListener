using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using Extractors;

namespace WebListener
{
    class EcopressOperator
    {
        private readonly MainViewModel _vm;

        private EcopressExtractor _ecopressExtractor;
        private EcopressRates _ecopressRates;

        public EcopressOperator(MainViewModel vm)
        {
            _vm = vm;
        }

        public void Start()
        {
            _ecopressExtractor = new EcopressExtractor();
            _ecopressRates = new EcopressRates();
            _ecopressRates.List = LoadLines();
            InitializeEcopressTimer();
        }

        private void InitializeEcopressTimer()
        {
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Tick += EcopressTimerTick;
            timer.Start();
        }

        private async void EcopressTimerTick(object sender, EventArgs e)
        {
            var ecopressRates = await _ecopressExtractor.GetRatesAsync();
            if (ecopressRates?.List == null)
                return;
            _vm.EcopressLastCheck = "Ecopress last check at " + DateTime.Now.ToLongTimeString();
            var listOfChangedLines = GetListOfChangedLines(_ecopressRates.List, ecopressRates.List);
            if (listOfChangedLines == null || listOfChangedLines.Count == 0) return;
            _ecopressRates = ecopressRates;
            SaveLines(ecopressRates.List);
            foreach (var line in listOfChangedLines)
            {
                var notify = new Changes
                {
                    MessageBlock = { Text = line.ToDisplayEcopress() }
                };
                notify.Show();
            }
        }

        private List<KomBankRates> LoadLines()
        {
            const string filename = @"ecopress.txt";
            var result = new List<KomBankRates>();

            if (!File.Exists(filename)) return result;
            var content = File.ReadAllLines(filename);
            result.AddRange(content.Select(line => new KomBankRates(line)));
            return result;
        }

        private void SaveLines(List<KomBankRates> list)
        {
            const string filename = @"ecopress.txt";

            var content = list.Select(line => line.ToFileString()).ToList();
            File.WriteAllLines(filename, content);
        }

        private List<KomBankRates> GetListOfChangedLines(List<KomBankRates> oldList, List<KomBankRates> newList)
        {
            return (from line in newList let oldLine = oldList.FirstOrDefault(l => l.Bank == line.Bank) where oldLine == null || line.IsDifferent(oldLine) select line).ToList();
        }
    }
}
