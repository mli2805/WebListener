using System.Collections.Generic;

namespace Extractors
{
    public class EcopressRates
    {
        public List<KomBankRates> List { get; set; }

        public EcopressRates()
        {
            List = new List<KomBankRates>();
        }

    }
}
