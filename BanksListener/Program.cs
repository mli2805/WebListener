using Autofac;
using BalisStandard;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using UtilsLib;

namespace BanksListener
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder().WithProduction();
            var container = builder.Build();

            StartKomBankPollers(container);
//            Banki24ArchiveManager.RunUpdatingInBackground();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void StartKomBankPollers(ILifetimeScope container)
        {
            var iniFile = container.Resolve<IniFile>();
            var logFile = container.Resolve<IMyLog>();
            logFile.AppendLine("BankListener pollers started");

            var googleDrive = PathFinder.GetGoogleDriveDirectory();
            string dataSourcePath;
            if (string.IsNullOrEmpty(googleDrive))
                dataSourcePath = @"..\bali.db";
            else
                dataSourcePath = googleDrive + @"\BanksListener\bali.db";
            logFile.AppendLine($"dataSourcePath: {dataSourcePath}");
            iniFile.Write(IniSection.Sqlite, IniKey.DbPath, dataSourcePath);
            new KomBanksPoller(container).Poll();
        }
    }
}
