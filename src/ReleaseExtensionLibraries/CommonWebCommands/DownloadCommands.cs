using RemotePlusLibrary.Extension.CommandSystem;
using System;
using System.Net;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusServer.Core;

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
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(client.DownloadString(args.Arguments[1].Value));
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (Exception ex)
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Unable to download file: {ex.Message}", BetterLogger.LogLevel.Error, "WebCommands");
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
    }
}
