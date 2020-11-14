using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using UtilsLib;

namespace BanksListener
{
    public class BalisSignalRHub : Hub
    {
        private readonly IMyLog _logFile;
        private readonly string _localIpAddress;
        private readonly Process _process;


        public BalisSignalRHub(IniFile iniFile, IMyLog logFile)
        {
            _logFile = logFile;
            _localIpAddress = iniFile.Read(IniSection.ClientLocalAddress, IniKey.Ip, "localhost");
            _process = Process.GetCurrentProcess();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType">This will be method name on Client</param>
        /// <param name="dataInJson"></param>
        /// <returns></returns>
        public async Task NotifyAll(string eventType, string dataInJson)
        {
            var tid = Thread.CurrentThread.ManagedThreadId;
            _logFile.AppendLine($"Thread id {tid}: Hub received {eventType} event from client with id = {Context.ConnectionId}");
            await Clients.All.SendAsync(eventType, dataInJson);
            var memory64 = _process.PrivateMemorySize64 / 1024;
            _logFile.AppendLine($"Process's private memory usage {memory64:0,0}K");
        }


        private string GetRemoteAddress()
        {
            var ip1 = Context.GetHttpContext().Connection.RemoteIpAddress.ToString();
            // browser started on the same pc as this service
            return ip1 == "::1" ? _localIpAddress : ip1;
        }

        public override async Task OnConnectedAsync()
        {
            _logFile.AppendLine($"SignalR Hub OnConnectedAsync: User {Context.User.Identity.Name}" +
                                $" connected from = {GetRemoteAddress()} assigned id {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            _logFile.AppendLine($"SignalR Hub OnDisconnectedAsync: User {Context.User.Identity.Name}" +
                                $" from = {GetRemoteAddress()} with id {Context.ConnectionId}");

            await base.OnDisconnectedAsync(new Exception("SignalR disconnected"));

        }
    }
}
