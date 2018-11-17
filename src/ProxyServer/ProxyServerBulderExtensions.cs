using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProxyServer.ProxyManager;

namespace ProxyServer
{
    public static class ProxyServerBulderExtensions
    {
        private static void InitializeCommands()
        {
            ProxyService.Commands.Add("proxySwitchServer", switchServer);
            ProxyService.Commands.Add("proxyHelp", help);
            ProxyService.Commands.Add("proxyViewServers", viewServers);
            ProxyService.Commands.Add("proxyRegister", register);
            ProxyService.Commands.Add("proxyResetStaticScript", resetStaticScript);
        }
        public static IServerBuilder AddDefaultProxyCommands(this IServerBuilder builder)
        {
            return builder.AddTask(InitializeCommands);
        }
    }
}
