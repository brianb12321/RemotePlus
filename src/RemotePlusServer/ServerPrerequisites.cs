using Logging;
using RemotePlusServer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer
{
    public static class ServerPrerequisites
    {
        public static void CheckPrivilleges()
        {
            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            WindowsPrincipal p = new WindowsPrincipal(wi);
            if (!p.IsInRole(WindowsBuiltInRole.Administrator))
            {
                ServerManager.Logger.AddOutput("The current logged in user is not part of the group \"Administrator\". This may cause certain operations to fail.", OutputLevel.Warning);
            }
        }
        public static void CheckNetworkInterfaces()
        {
            bool foundFlag = false;
            NetworkInterface[] adapt = NetworkInterface.GetAllNetworkInterfaces();
            if (adapt.Length == 0)
            {
                ServerManager.Logger.AddOutput("Unable to find a network adapter. The server requires at least one adapter.", OutputLevel.Error);
            }
            else
            {
                foreach (NetworkInterface nif in adapt)
                {
                    if (nif.GetIPProperties().GetIPv4Properties() != null)
                    {
                        if (!nif.GetIPProperties().GetIPv4Properties().IsDhcpEnabled)
                        {
                            foundFlag = true;
                            break;
                        }
                    }
                }
                if (!foundFlag)
                {
                    ServerManager.Logger.AddOutput("You should have at least one network adapter that has a static IP. This could cause the client fail to connect to the server.", OutputLevel.Info);
                }
            }
        }

        internal static void CheckSettings()
        {
            if (ServerManager.DefaultSettings.LoggingSettings.LogOnShutdown)
            {
                ServerManager.Logger.AddOutput(new LogItem(OutputLevel.Info, "NOTE: Logging is enabled for this application.", ServerManager.Logger.DefaultFrom) { Color = ConsoleColor.Cyan });
            }
            if (ServerManager.DefaultSettings.LoggingSettings.CleanLogFolder)
            {
                ServerManager.Logger.AddOutput(new LogItem(OutputLevel.Info, $"NOTE: Logs will be cleaned out when there are {ServerManager.DefaultSettings.LoggingSettings.LogFileCountThreashold} logs in the logs foleder.", ServerManager.Logger.DefaultFrom) { Color = ConsoleColor.Cyan });
                CheckLogCount();
            }
        }
        private static void CheckLogCount()
        {
            if (Directory.GetFiles("ServerLogs").Length >= ServerManager.DefaultSettings.LoggingSettings.LogFileCountThreashold)
            {
                ServerManager.Logger.AddOutput(new LogItem(OutputLevel.Info, "IMPORTANT ACTION: The server logs threashold has been reached. The server logs folder will be cleared.", ServerManager.Logger.DefaultFrom) { Color = ConsoleColor.Magenta });
                foreach (string fileToBeDeleted in Directory.GetFiles("ServerLogs"))
                {
                    File.Delete(fileToBeDeleted);
                }
                ServerManager.Logger.AddOutput(new LogItem(OutputLevel.Info, "IMPORTANT ACTION COMPLETE: The ServerLogs folder has been purged.", ServerManager.Logger.DefaultFrom) { Color = ConsoleColor.Green });
            }
        }
    }
}