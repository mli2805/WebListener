using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using UtilsLib;

namespace BanksListener
{
    public class SignalRHub : Hub
    {
        private readonly IMyLog _logFile;
        private readonly string _localIpAddress;

        public SignalRHub(IniFile iniFile, IMyLog logFile)
        {
            _logFile = logFile;
            _localIpAddress = iniFile.Read(IniSection.ClientLocalAddress, IniKey.Ip, "localhost");
        }

        public async Task NotifyAll(string eventType, string dataInJson)
        {
            _logFile.AppendLine($"Hub received {eventType} event");
            await Clients.All.SendAsync(eventType, dataInJson);
        }

        private string GetRemoteAddress()
        {
            var ip1 = Context.GetHttpContext().Connection.RemoteIpAddress.ToString();
            // browser started on the same pc as this service
            return ip1 == "::1" ? _localIpAddress : ip1;
        }

        public override async Task OnConnectedAsync()
        {
            _logFile.AppendLine($"OnConnectedAsync ClientIp = {GetRemoteAddress()}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            _logFile.AppendLine($"OnDisconnectedAsync ClientIp = {GetRemoteAddress()}");

            await base.OnDisconnectedAsync(new Exception("SignalR disconnected"));
          
        }
    }
}
