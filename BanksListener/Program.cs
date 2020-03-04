using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace BanksListener
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Task.Factory.StartNew(Poll);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static async void Poll()
        {
            while (true)
            {
                var rate = await Fetch();
                var unused = await Persist(rate);
                await Task.Delay(15000);

            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static async Task<KomBankRatesLine> Fetch()
        {
            var belgaz = new BelgazMobi();
            KomBankRatesLine rate;
            while (true)
            {
                rate = await belgaz.GetRatesLineAsync();
                if (rate != null) break;
                await Task.Delay(2000);
            }

            return rate;
        }

        private static async Task<int> Persist(KomBankRatesLine rate)
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
