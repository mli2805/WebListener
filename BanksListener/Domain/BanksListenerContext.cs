using Microsoft.EntityFrameworkCore;

namespace BanksListener
{
    public class BanksListenerContext : DbContext
    {
        public DbSet<KomBankRatesLine> KomBankRates { get; set; }
        public DbSet<BelStockArchiveOneCurrencyDay> BelStockArchive { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var googleDrive = PathFinder.GetGoogleDriveDirectory();
            string dataSourcePath;
            if (string.IsNullOrEmpty(googleDrive))
                dataSourcePath = "bali.db";
            else
                dataSourcePath = googleDrive + @"\BanksListener\bali.db";
            var connectionString = $"Data Source={dataSourcePath}";
            options.UseSqlite(connectionString);
        }
    }
}
