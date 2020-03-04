using Microsoft.EntityFrameworkCore;

namespace BanksListener
{
    public class BanksListenerContext : DbContext
    {
        public DbSet<KomBankRatesLine> KomBankRates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=bali.db");
    }
}
