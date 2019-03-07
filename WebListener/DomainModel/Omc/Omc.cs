using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using WebListener.Properties;

namespace WebListener
{
    public class Omc : WebExtractionResult, INotifyPropertyChanged
    {
        public string FromDate { get; set; }
        public Dictionary<string, OmcMetal> Metals { get; set; }

        public Omc()
        {
        }

        public Omc(string str)
        {
            if (str == null) return;
            var ss = str.Split();
            FromDate = ss[0].Replace('_', ' ');
            Metals = new Dictionary<string, OmcMetal>
            {
                {"ЗОЛОТО", new OmcMetal(ss[2], ss[3], ss[4], ss[5])},
                {"СЕРЕБРО", new OmcMetal(ss[7], ss[8], ss[9], ss[10])},
                {"ПЛАТИНА", new OmcMetal(ss[12], ss[13], ss[14], ss[15])},
                {"ПАЛЛАДИЙ", new OmcMetal(ss[17], ss[18], ss[19], ss[20])}
            };
        }

        public string ToFileString()
        {
            StringBuilder result = new StringBuilder(FromDate.Replace(' ', '_'));
            foreach (var metal in Metals)
            {
                result.AppendFormat(" {0} {1}", metal.Key, metal.Value.ToFileString());
            }
            return result.ToString();
        }

        public bool Equals(Omc omc)
        {
            if (FromDate != omc.FromDate) return false;
            foreach (var metal in Metals)
            {
                if (!omc.Metals[metal.Key].Equals(metal.Value)) return false;
            }
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override void Apply()
        {
            throw new NotImplementedException();
        }
    }
}