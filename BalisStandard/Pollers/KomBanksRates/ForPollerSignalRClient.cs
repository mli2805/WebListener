using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UtilsLib;

namespace BalisStandard
{
    public class ForPollerSignalRClient
    {
        private readonly IniFile _iniFile;
        private readonly IMyLog _logFile;
        private HubConnection _connection;
       
        public ForPollerSignalRClient(IniFile iniFile, IMyLog logFile)
        {
            _iniFile = iniFile;
            _logFile = logFile;
        }

        public void NotifyAll(string eventType, string dataInJson)
        {
            _connection.InvokeAsync("NotifyAll", eventType, dataInJson);
        }

        public async void Start()
        {
            try
            {
                var baliApiUrl = _iniFile.Read(IniSection.General, IniKey.BaliApiUrl, "localhost:11082");
                string url = $"http://{baliApiUrl}/balisSignalRHub";
                _logFile.AppendLine($"SignalR connection to {url}");
                _connection = new HubConnectionBuilder()
                    .WithUrl(url)
                    .WithAutomaticReconnect()
                    .Build();

                _connection.Closed += async (error) =>
                {
                    await Task.Delay(1);
                    _logFile.AppendLine($"ForPollerSignalRClient closed: {error.Message}");
//                    _logFile.AppendLine("ForPollerSignalRClient connection was closed. Restarting...");
//                    await Task.Delay(new Random().Next(0, 5) * 1000);
//                    await _connection.StartAsync();
//                    _logFile.AppendLine($"SignalR connection state is {_connection.State}");
                };
             
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