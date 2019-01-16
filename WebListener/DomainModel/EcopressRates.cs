using System.Collections.Generic;

namespace WebListener.DomainModel
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
