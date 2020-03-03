using System;
using System.Threading.Tasks;
using Extractors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BanksListener
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Task.Factory.StartNew(Fetch);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static async void Fetch()
        {
            var belgaz = new BelgazMobi();
            var rate = await belgaz.GetRatesLineAsync();
            Console.WriteLine(rate);
        }
    }
}
