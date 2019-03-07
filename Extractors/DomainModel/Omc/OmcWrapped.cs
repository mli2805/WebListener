using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Extractors.Properties;

namespace Extractors
{
    public class OmcWrapped : INotifyPropertyChanged
    {
        private readonly Omc _omc;
        private string _lastChecked;
        public string FromDate => $"{_omc.FromDate}";
        public string Gold => GoldToString();

        public string LastChecked   
        {
            get { return _lastChecked; }
            set
            {
                if (value.Equals(_lastChecked)) return;
                _lastChecked = value;
                OnPropertyChanged();
            }
        }

        public OmcWrapped(Omc omc)
        {
            _omc = omc;
        }

        private string GoldToString()
        {
            var gold = _omc.Metals["ЗОЛОТО"];
            return $"     {gold.BankBuyUsd} - {gold.BankSellUsd} ({Math.Round((1 - gold.BankBuyUsd / gold.BankSellUsd) * 100, 2)}%)" +
                   $"\n {gold.BankBuyByn:0,0.0000} - {gold.BankSellByn:0,0.0000} ({Math.Round((1 - gold.BankBuyByn / gold.BankSellByn) * 100, 2)}%)" +
                   $"\n   {gold.BankBuyByn / gold.BankBuyUsd:#,0.0000} - {gold.BankSellByn / gold.BankSellUsd:#,0.0000}  ";
        }

        public bool IsOmcChanged(Omc omc)
        {
            return !_omc.Equals(omc);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}