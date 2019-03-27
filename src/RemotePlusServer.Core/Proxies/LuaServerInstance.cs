using System;
using System.ServiceModel;
using System.Text;
using BetterLogger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Scripting;

namespace RemotePlusServer.Core.Proxies
{
    /// <summary>
    /// Provides functions that allows a script to close the server and get global information about the server.
    /// </summary>
    public class PythonServerInstance
    {
        public string CurrentPath
        {
            get
            {
                return ServerManager.ServerRemoteService.RemoteInterface.CurrentPath;
            }
        }
        public ClientInstance Client = new ClientInstance();
        public void showServerInformation()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("ServerName: RemotePlus");
            builder.AppendLine($"ServerVersion: {ServerManager.DefaultSettings.ServerVersion}");
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(builder.ToString(), LogLevel.Info);
        }
        public void printToServerConsole(string message)
        {
            Console.WriteLine(message);
        }
        public void logToServerConsole(string message, LogLevel LogLevel)
        {
            GlobalServices.Logger.Log(message, LogLevel, "Scripting Engine");
        }
        public void createFault(string message)
        {
            throw new FaultException(message);
        }
        public string getServerLog()
        {
            return Console.In.ReadToEnd();
        }
    }
}