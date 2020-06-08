using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UtilsLib;

namespace BalisWpf
{
    public class FtSignalRClient
    {
        private readonly IMyLog _logFile;
        private HubConnection connection;
        private readonly string _webApiUrl;

        public FtSignalRClient(IMyLog logFile)
        {
            _logFile = logFile;
            _webApiUrl = $"http://localhost:8012/balisSignalRHub";
        }

        private void Build()
        {
            connection = new HubConnectionBuilder()
                .WithUrl(_webApiUrl)
                .Build();

            connection.Closed += async (error) =>
            {
                _logFile.AppendLine("FtSignalRClient connection was closed. Restarting...");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
        }

        public async Task NotifyAll(string eventType, string dataInJson)
        {
            try
            {
                var isConnected = await IsSignalRConnected();
                if (isConnected)
                    await connection.InvokeAsync("NotifyAll", eventType, dataInJson);
            }
            catch (Exception ex)
            {
                _logFile.AppendLine($"FtSignalRClient: {eventType} " + ex.Message);
            }
        }

        public async Task<bool> IsSignalRConnected()
        {
            try
            {
                if (connection == null)
                {
                    _logFile.AppendLine($"Build signalR connection to {_webApiUrl}");
                    Build();
                    _logFile.AppendLine($"SignalR connection state is {connection.State}");
                    await Task.Delay(2000);


                    _logFile.AppendLine($"Start signalR connection to {_webApiUrl}");
                    await connection.StartAsync();
                    _logFile.AppendLine($"SignalR connection state is {connection.State}");
                    await Task.Delay(2000);
                }
                else if (connection.State != HubConnectionState.Connected)
                {
                    _logFile.AppendLine($"Start signalR connection to {_webApiUrl}");
                    await connection.StartAsync();
                    await Task.Delay(2000);
                }

                return true;
            }
            catch (Exception e)
            {
                _logFile.AppendLine($"FtSignalRClient Start connection: " + e.Message);
                return false;
            }
        }
    }
}
