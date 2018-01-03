using RemotePlusLibrary.Extension.CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using RemotePlusServer;
using Logging;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;

namespace CommonWebCommands
{
    public static class DownloadCommands
    {
        [CommandHelp("Downloads a file from the internet and displays it in the console.")]
        public static CommandResponse downloadWeb(CommandRequest args, CommandPipeline pipe)
        {
            WebClient client = new WebClient();
            try
            {
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(client.DownloadString(args.Arguments[1].Value));
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (Exception ex)
            {
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(OutputLevel.Error, $"Unable to download file: {ex.Message}", "WebCommands"));
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
    }
}
