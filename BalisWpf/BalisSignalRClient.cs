using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UtilsLib;

namespace BalisWpf
{
    public class BalisSignalRClient
    {
        private const string _balisWebApiUrl = "http://localhost:8012/balisSignalRHub";
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
                _connection.On<string, string>("ReceiveMessage", (user, message) =>
                {
                        var newMessage = $"{user}: {message}";
                        _logFile.AppendLine(newMessage);
                });

                await _connection.StartAsync();
                _logFile.AppendLine($"SignalR connection state is {_connection.State}");
            }
            catch (Exception e)
            {
                _logFile.AppendLine($"SignalR client start failed: {e.Message}");
            }
        }

    
    }
}
