using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Logging;
using RemotePlusLibrary.Scripting;

namespace RemotePlusServer.Proxies
{
    /// <summary>
    /// Provides functions that allows a script to close the server and get global information about the server.
    /// </summary>
    public class LuaServerInstance
    {
        [IndexScriptObject]
        public string CurrentPath
        {
            get
            {
                return ServerManager.DefaultService.Remote.CurrentPath;
            }
        }
        [IndexScriptObject]
        public ClientInstance Client = new ClientInstance();
        [IndexScriptObject]
        public void showServerInformation()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("ServerName: RemotePlus");
            builder.AppendLine($"ServerVersion: {ServerManager.DefaultSettings.ServerVersion}");
            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, builder.ToString()));
        }
        [IndexScriptObject]
        public void printToServerConsole(string message)
        {
            Console.WriteLine(message);
        }
        [IndexScriptObject]
        public void logToServerConsole(string message, int outputLevel)
        {
            Logging.OutputLevel level = Logging.OutputLevel.Info;
            switch(outputLevel)
            {
                case 0:
                    level = Logging.OutputLevel.Info;
                    break;
                case 1:
                    level = Logging.OutputLevel.Warning;
                    break;
                case 2:
                    level = Logging.OutputLevel.Error;
                    break;
                case 3:
                    level = Logging.OutputLevel.Debug;
                    break;
                default:
                    throw new Exception("Invalid OutputLevel. Please select a level in the range of 0 through 3");
            }
            ServerManager.Logger.AddOutput(message, level, ScriptBuilder.SCRIPT_LOG_CONSTANT);
        }
        [IndexScriptObject]
        public void createFault(string message)
        {
            throw new FaultException(message);
        }
        [IndexScriptObject]
        public string getServerLog()
        {
            StringBuilder sb = new StringBuilder();
            foreach(LogItem li in ServerManager.Logger.buffer)
            {
                sb.AppendLine(li.ToString());
            }
            return sb.ToString();
        }
    }
}