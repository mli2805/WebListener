using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BanksListener
{
    public class Program
    {
        public static void Main(string[] args)
        {
            File.WriteAllText($@"c:\tmp\{DateTime.Now:yyyy.MM.dd-HH.mm.ss}.txt", DateTime.Now.ToString("F"));

            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://*:11082");
                })
                .UseWindowsService();
    }
}
