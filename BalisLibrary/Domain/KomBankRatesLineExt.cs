using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BalisLibrary
{
    public static class KomBankRatesLineExt
    {
        public static async Task<int> Persist(this KomBankRatesLine rate)
        {
            using (BanksListenerContext db = new BanksListenerContext())
            {
                var last = await db.KomBankRates.Where(l => l.Bank == rate.Bank).OrderBy(c=>c.LastCheck).LastOrDefaultAsync();
                if (last == null || last.IsDifferent(rate))
                    db.KomBankRates.Add(rate);
                else 
                    last.LastCheck = DateTime.Now;
                return await db.SaveChangesAsync();
            }
        }
    }
}