using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UtilsLib;

namespace BalisStandard
{
    public class BalisSignalRClient : INotifyPropertyChanged
    {
        private const string _balisWebApiUrl = "http://localhost:8012/balisSignalRHub";
        private readonly IMyLog _logFile;
        private HubConnection _connection;
        private Tuple<string, string> _eventProperty;

        public Tuple<string, string> EventProperty
        {
            get => _eventProperty;
            set
            {
                if (Equals(value, _eventProperty)) return;
                _eventProperty = value;
                OnPropertyChanged();
            }
        }

        public BalisSignalRClient(IMyLog logFile)
        {
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
                _connection = new HubConnectionBuilder().WithUrl(_balisWebApiUrl).Build();

                _connection.Closed += async (error) =>
                {
                    _logFile.AppendLine("BalisSignalRClient connection was closed. Restarting...");
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await _connection.StartAsync();
                };
                _connection.On<string>("RateChanged", (dataInJson) =>
                {
                    EventProperty = new Tuple<string, string>("RateChanged", dataInJson);
                    var newMessage = $"RateChanged: {dataInJson}";
                    _logFile.AppendLine(newMessage);
                });
                _connection.On<string>("TheSameRate", (dataInJson) =>
                {
                    EventProperty = new Tuple<string, string>("TheSameRate", dataInJson);
//                    var newMessage = $"TheSameRate: {dataInJson}";
//                    _logFile.AppendLine(newMessage);
                });

                await _connection.StartAsync();
                _logFile.AppendLine($"SignalR connection state is {_connection.State}");
            }
            catch (Exception e)
            {
                _logFile.AppendLine($"SignalR client start failed: {e.Message}");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
