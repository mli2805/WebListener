using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Threading;
using Extractors;

namespace WebListener
{
    class BpsOmcOperator
    {
        private readonly MainViewModel _vm;
        public BpsOmcOperator(MainViewModel vm)
        {
            _vm = vm;
        }

        public void InitializeOmcTimer()
        {
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 15);
            timer.Tick += BpsOmcTimerTick;
            timer.Start();

        }
        private async void BpsOmcTimerTick(object sender, EventArgs e)
        {
            var result = await new BpsOmcExtractor().GetOmcAsync();
            if (result != null)
                ApplyOmcLine((Omc)result, _vm);
        }

        public void ApplyOmcLine(Omc omc, MainViewModel vm)
        {
            if (omc.FromDate == "error") return;

            if (vm.OmcWrapped == null || vm.OmcWrapped.IsOmcChanged(omc))
            {
                SaveLine(omc);
                vm.OmcWrapped = new OmcWrapped(omc) { LastChecked = DateTime.Now.ToString("HH:mm:ss") };
            }
            else
            {
                vm.OmcWrapped.LastChecked = DateTime.Now.ToString("HH:mm:ss");
            }
        }
        private void SaveLine(Omc currentOmc)
        {
            var content = new List<string> { currentOmc.ToFileString() };
            File.AppendAllLines(@"omc_bps.txt", content);
        }

        public void GetLast(MainViewModel vm)
        {
            vm.OmcWrapped = null;
            const string filename = @"omc_bps.txt";
            if (!File.Exists(filename)) return;
            var content = File.ReadAllLines(filename);
            foreach (var line in content)
            {
                vm.OmcWrapped = new OmcWrapped(new Omc(line));
            }
        }

    }
}
