using Microsoft.EntityFrameworkCore;

namespace BalisStandard
{
    public class BanksListenerContext : DbContext
    {
        private readonly string _dbPath;
        public DbSet<KomBankRatesLine> KomBankRates { get; set; }
        public DbSet<BelStockArchiveOneCurrency> Banki24Archive { get; set; }

        public BanksListenerContext(string dbPath)
        {
            _dbPath = dbPath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connectionString = $"Data Source={_dbPath}";
            options.UseSqlite(connectionString);
        }
    }
}
