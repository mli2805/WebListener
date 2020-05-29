using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UtilsLib;

namespace BalisWpf
{
    public class BalisSignalRClient
    {
        private const string _balisWebApiUrl = "http://localhost:1234/balisSignalRHub";
        private readonly IMyLog _logFile;
        private HubConnection _connection;

        public BalisSignalRClient(IMyLog logFile)
        {
            _logFile = logFile;
        }

        public async void Start()
        {
            try
            {
                _connection = new HubConnectionBuilder().WithUrl(_balisWebApiUrl).Build();

                _connection.Closed += async (error) =>
                {
                    _logFile.AppendLine("BalisSignalRClient connection was closed. Restarting...");
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await _connection.StartAsync();
                };

                await _connection.StartAsync();
                _logFile.AppendLine($"SignalR connection state is {_connection.State}");
            }
            catch (Exception e)
            {
                _logFile.AppendLine($"SignalR client start failed: {e.Message}");
            }
        }

        public async Task NotifyAll(string eventType, string dataInJson)
        {
            try
            {
                await _connection.InvokeAsync("NotifyAll", eventType, dataInJson);
            }
            catch (Exception ex)
            {
                _logFile.AppendLine($"FtSignalRClient: {eventType} " + ex.Message);
            }
        }
    }
}
