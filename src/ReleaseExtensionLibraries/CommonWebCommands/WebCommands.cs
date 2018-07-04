using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusServer;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusServer.Core;

namespace CommonWebCommands
{
    public static class WebCommands
    {
        [CommandHelp("Starts a new chrome seassion.")]
        public static CommandResponse chrome(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                ServerManager.ServerRemoteService.RemoteInterface.RunProgram("cmd.exe", $"/c \"start chrome.exe {args.Arguments[1]}\"");
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("chrome started", BetterLogger.LogLevel.Info, "WebCommands");
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch
            {
                return new CommandResponse((int)CommandStatus.Fail);
                throw;
            }
        }
        [CommandHelp("Starts a new internet explorer seassion.")]
        public static CommandResponse ie(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                ServerManager.ServerRemoteService.RemoteInterface.RunProgram("cmd.exe", $"/c \"start iexplore.exe {args.Arguments[1]}\"");
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("chrome started", BetterLogger.LogLevel.Info, "WebCommands");
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch
            {
                return new CommandResponse((int)CommandStatus.Fail);
                throw;
            }
        }
        [CommandHelp("Starts a new Opera seassion")]
        public static CommandResponse opera(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                ServerManager.ServerRemoteService.RemoteInterface.RunProgram("cmd.exe", $"/c \"start opera.exe {args.Arguments[1]}\"");
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Opera started", BetterLogger.LogLevel.Info, "WebCommands");
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch
            {
                return new CommandResponse((int)CommandStatus.Fail);
                throw;
            }
        }
        [CommandHelp("Starts a new Firefox seassion")]
        public static CommandResponse firefox(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                ServerManager.ServerRemoteService.RemoteInterface.RunProgram("cmd.exe", $"/c \"start firefox.exe {args.Arguments[1]}\"");
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Firefox started", BetterLogger.LogLevel.Info, "WebCommands");
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch
            {
                return new CommandResponse((int)CommandStatus.Fail);
                throw;
            }
        }
    }
}