using System;
using System.Collections.ObjectModel;
using Caliburn.Micro;

namespace BalisWpf
{
    public class ChangesViewModel : Screen
    {
        public ObservableCollection<ChangesLineVm> Rows { get; set; } = new ObservableCollection<ChangesLineVm>();

        public bool IsOpen { get; set; }

        protected override void OnViewLoaded(object view)
        {
            IsOpen = true;
            DisplayName = "Changes";
        }

        public void AddNewLine(KomBankRateVm line)
        {
            Rows.Add(new ChangesLineVm()
            {
                Bank = line.Bank,
                Timestamp = line.StartedFrom,
                UsdRate = line.Usd,
            });
        }

        public override void CanClose(Action<bool> callback)
        {
            Rows.Clear();
            IsOpen = false;
            base.CanClose(callback);
        }
    }
}
