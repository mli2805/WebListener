using BalisStandard;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BanksListener
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new KomBanksPoller().Poll();
         //   new TradingViewPoller(new TradingViewCurrentRates()).Poll(); // on client

            Banki24ArchiveManager.RunUpdatingInBackground();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
