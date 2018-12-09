using RemotePlusLibrary.Extension.CommandSystem;
using System;
using System.Net;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusServer.Core;
using BetterLogger;
using System.IO;

namespace CommonWebCommands
{
    public class DownloadCommands : StandordCommandClass
    {
        public override void AddCommands()
        {
            Commands.Add("downloadWeb", downloadWeb);
        }

        [CommandHelp("Downloads a file from the internet and displays it in the console.")]
        public  CommandResponse downloadWeb(CommandRequest args, CommandPipeline pipe)
        {
            WebClient client = new WebClient();
            try
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(client.DownloadString(args.Arguments[1].ToString()));
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
