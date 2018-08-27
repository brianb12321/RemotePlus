using BetterLogger;
using RemotePlusLibrary;
using RemotePlusServer.Core;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Principal;

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
                GlobalServices.Logger.Log("The current logged in user is not part of the group \"Administrator\". This may cause certain operations to fail.", LogLevel.Warning);
            }
        }
        public static void CheckNetworkInterfaces()
        {
            bool foundFlag = false;
            NetworkInterface[] adapt = NetworkInterface.GetAllNetworkInterfaces();
            if (adapt.Length == 0)
            {
                GlobalServices.Logger.Log("Unable to find a network adapter. The server requires at least one adapter.", LogLevel.Error);
            }
            else
            {
                foreach (NetworkInterface nif in adapt)
                {
                    if (nif.GetIPProperties().GetIPv4Properties()?.IsDhcpEnabled == false)
                    {
                        foundFlag = true;
                        break;
                    }
                }
                if (!foundFlag)
                {
                    GlobalServices.Logger.Log("You should have at least one network adapter that has a static IP. This could cause the client to fail to connect to the server.", LogLevel.Warning);
                }
            }
        }

        internal static void CheckSettings()
        {
            if (ServerManager.DefaultSettings.LoggingSettings.LogOnShutdown)
            {
                GlobalServices.Logger.Log("NOTE: Logging is enabled for this application.", LogLevel.Info);
            }
            if (ServerManager.DefaultSettings.LoggingSettings.CleanLogFolder)
            {
                GlobalServices.Logger.Log($"NOTE: Logs will be cleaned out when there are {ServerManager.DefaultSettings.LoggingSettings.LogFileCountThreashold} logs in the logs folder.", LogLevel.Info);
                CheckLogCount();
            }
        }
        private static void CheckLogCount()
        {
            if (Directory.GetFiles("ServerLogs").Length >= ServerManager.DefaultSettings.LoggingSettings.LogFileCountThreashold)
            {
                GlobalServices.Logger.Log("IMPORTANT ACTION: The server logs threshold has been reached. The server logs folder will be cleared.", LogLevel.Info);
                foreach (string fileToBeDeleted in Directory.GetFiles("ServerLogs"))
                {
                    File.Delete(fileToBeDeleted);
                }
                GlobalServices.Logger.Log("IMPORTANT ACTION COMPLETE: The ServerLogs folder has been purged.", LogLevel.Info);
            }
        }
    }
}