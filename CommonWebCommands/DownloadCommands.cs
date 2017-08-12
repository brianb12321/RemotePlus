using RemotePlusLibrary.Extension.CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using RemotePlusServer;
using Logging;

namespace CommonWebCommands
{
    public static class DownloadCommands
    {
        [CommandHelp("Downloads a file from the internet and displays it in the console.")]
        public static int downloadWeb(string[] args)
        {
            WebClient client = new WebClient();
            try
            {
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(client.DownloadString(args[1]));
                return (int)CommandStatus.Success;
            }
            catch (Exception ex)
            {
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(OutputLevel.Error, $"Unable to download file: {ex.Message}", "WebCommands"));
                return (int)CommandStatus.Fail;
            }
        }
    }
}
