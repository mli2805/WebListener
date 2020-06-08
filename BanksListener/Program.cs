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
            StartKomBankPollers();
//            Banki24ArchiveManager.RunUpdatingInBackground();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void StartKomBankPollers()
        {
            var iniFile = new IniFile();
            iniFile.AssignFile("baliSelf.ini");

            var logFile = new LogFile(iniFile);
            logFile.AssignFile("baliSelf.log");
            logFile.AppendLine("BankListener pollers started");

            var googleDrive = PathFinder.GetGoogleDriveDirectory();
            string dataSourcePath;
            if (string.IsNullOrEmpty(googleDrive))
                dataSourcePath = @"..\bali.db";
            else
                dataSourcePath = googleDrive + @"\BanksListener\bali.db";
            logFile.AppendLine($"dataSourcePath: {dataSourcePath}");
            iniFile.Write(IniSection.Sqlite, IniKey.DbPath, dataSourcePath);
            new KomBanksPoller(logFile, dataSourcePath).Poll();
        }
    }
}
