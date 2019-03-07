using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using WebListener.DomainModel;

namespace WebListener.Functions
{
    class KomBankOperator
    {
        private readonly MainViewModel _vm;
        public KomBankOperator(MainViewModel vm)
        {
            _vm = vm;
        }

        public void InitializeKomBanksTimers()
        {
            var list = Enum.GetValues(typeof(KomBank)).Cast<KomBank>();
            foreach (var bankName in list)
                InitializeTimer(KomBankTimerTick, bankName);
        }
        private void InitializeTimer(EventHandler kombankTimerTick, KomBank bank)
        {
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 15);
            timer.Tick += kombankTimerTick;
            timer.Tag = bank;
            timer.Start();
        }
        private async void KomBankTimerTick(object sender, EventArgs e)
        {
            try
            {
                var result = await GetExtractor((KomBank)((DispatcherTimer)sender).Tag);
                var bankLines = GetCollection(_vm, result.Bank);
                ApplyKomBankRateLine(result, bankLines);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public Task<KomBankRates> GetExtractor(KomBank bank)
        {
            switch (bank)
            {
                case KomBank.Бгпб: return new BelgazExtractor().GetRatesLineAsync();
                case KomBank.Биб: return new BibExtractor().GetRatesLineAsync();
                case KomBank.Ммб: return new MmbankExtractor().GetRatesLineAsync();
                case KomBank.Бпс: return new BpsExtractor().GetRatesLineAsync();
                case KomBank.Приор: return new PriorExtractor().GetRatesLineAsync();
                default:
                    return null;
            }
        }
        public void ApplyKomBankRateLine(KomBankRates result, ObservableCollection<KomBankRates> rows)
        {
            if (result.StartedFrom == "error") return;

            var oldLine = rows.LastOrDefault();
            if (oldLine == null)
                ApplyFirstLine(result, rows);
            else if (result.IsDifferent(oldLine))
            {
                if (result.Bank == "БИБ") 
                    result.StartedFrom = DateTime.Now.ToString("g");
                ApplyNewLine(result, rows);
            }
            else
                oldLine.LastCheck = result.LastCheck;
        }

        private void ApplyFirstLine(KomBankRates line, ObservableCollection<KomBankRates> rows)
        {
            rows.Add(line);
            new KomBankFileOperator().SaveLine(line);
        }

        private void ApplyNewLine(KomBankRates line, ObservableCollection<KomBankRates> rows)
        {
            rows.Add(line);
            new KomBankFileOperator().ReWriteSet(rows);
            var notify = new Views.Changes
            {
                MessageBlock = { Text = line.Bank + " " + line.StartedFrom }
            };
            notify.Show();
        }
        public ObservableCollection<KomBankRates> GetCollection(MainViewModel vm, string bank)
        {
            switch (bank)
            {
                case "БГПБ": return vm.RowsBelGaz;
                case "БИБ": return vm.RowsBib;
                case "ММБ": return vm.RowsMoMi;
                case "БПС": return vm.RowsBps;
                case "Приор": return vm.RowsPrior;
                default:
                    return null;
            }
        }
    }
}
