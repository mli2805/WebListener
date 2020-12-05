using System.Threading;
using System.Threading.Tasks;
using BalisStandard;
using Microsoft.Extensions.Hosting;
using UtilsLib;

namespace BanksListener
{
    public class KomBanksHostedService : IHostedService
    {
        private readonly IMyLog _logFile;
        private readonly KomBanksPoller _komBanksPoller;

        public KomBanksHostedService(IMyLog logFile, KomBanksPoller komBanksPoller)
        {
            _logFile = logFile;
            _komBanksPoller = komBanksPoller;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logFile.AppendLine("we are starting kom banks poller");
            _komBanksPoller.StartThreads();
            await Task.Delay(1, cancellationToken);
        }
      
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(1, cancellationToken);
            _logFile.AppendLine("we are leaving background task with kom banks poller");
        }

    }
}