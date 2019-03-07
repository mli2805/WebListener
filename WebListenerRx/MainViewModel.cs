using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Extractors;

namespace WebListenerRx
{
    public class MainViewModel : PropertyChangedBase
    {
        public ObservableCollection<string> Rows { get; set; } = new ObservableCollection<string>();

        public void LoadHistory()
        {
            var fileOperator = new KomBankFileOperator();
            foreach (KomBank2 komBank in Enum.GetValues(typeof(KomBank2)))
            {
                var allOfBank = fileOperator.LoadLines(komBank).ToList();
                foreach (var line in allOfBank.Skip(allOfBank.Count - 15))
                {
                    Application.Current.Dispatcher.Invoke(() => Rows.Add(line.ToFileString()));
                }
            }
        }
    }
}
