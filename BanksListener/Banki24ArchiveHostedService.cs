using System.Threading;
using System.Threading.Tasks;
using BalisStandard;
using Microsoft.Extensions.Hosting;
using UtilsLib;

namespace BanksListener
{
    public class Banki24ArchiveHostedService : IHostedService
    {
        private readonly IMyLog _logFile;
        private readonly Banki24ArchiveManager _banki24ArchiveManager;

        public Banki24ArchiveHostedService(IMyLog logFile, Banki24ArchiveManager banki24ArchiveManager)
        {
            _logFile = logFile;
            _banki24ArchiveManager = banki24ArchiveManager;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logFile.AppendLine("we are starting banki24 archive manager");
            _banki24ArchiveManager.StartThread();
            await Task.Delay(1, cancellationToken);

        }
      
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(1, cancellationToken);
            _logFile.AppendLine("we are leaving background task with banki24 archive manager");
        }


        /*
        // https://www.taithienbo.com/hosting-a-background-task-in-an-asp-net-core-application-running-on-iis/
        public Task StartAsync(CancellationToken cancellationToken) 
        {
            // Invoke the DoWork method every 5 seconds. 
            _timer = new Timer(callback: async o => await DoWork(o), 
                state: null, dueTime: TimeSpan.FromSeconds(0), 
                period: TimeSpan.FromSeconds(JobIntervalInSecs));
            return Task.CompletedTask;
        }
        */
    }
}
