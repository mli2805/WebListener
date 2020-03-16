using System.ComponentModel;
using System.Runtime.CompilerServices;
using BalisWpf.Properties;

namespace BalisWpf
{
    public class ShellVm : INotifyPropertyChanged
    {
        public string Test { get; set; }

        private string _voo;
        private string _eurUsd;

        public string Voo
        {
            get => _voo;
            set
            {
                if (value == _voo) return;
                _voo = value;
                OnPropertyChanged();
            }
        }

        public string EurUsd
        {
            get => _eurUsd;
            set
            {
                if (value == _eurUsd) return;
                _eurUsd = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}