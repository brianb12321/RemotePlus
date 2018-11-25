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
using System.IO;
using BetterLogger;

namespace CommonWebCommands
{
    public class WebCommands : StandordCommandClass
    {
        private ILogFactory _logger;
        public WebCommands(ILogFactory logger)
        {
            _logger = logger;
        }
        public override void AddCommands()
        {
            _logger.Log("Adding Chrome", LogLevel.Info, "WebCommands");
            CheckChrome();
            Commands.Add("chrome", chrome);
            _logger.Log("Adding Internet Explore", LogLevel.Info, "WebCommands");
            CheckIE();
            Commands.Add("ie", ie);
            _logger.Log("Adding Opera", LogLevel.Info, "WebCommands");
            CheckOpera();
            Commands.Add("opera", opera);
            _logger.Log("Adding Firefox", LogLevel.Info, "WebCommands");
            CHeckFirefox();
            Commands.Add("firefox", firefox);
        }
        void CheckIE()
        {
            if (!File.Exists(@"C:\Program Files\Internet Explorer\iexplore.exe"))
            {
                _logger.Log("Internet explore is not installed on the server.", LogLevel.Warning, "WebCommands");
            }
        }
        void CheckChrome()
        {
            if (!File.Exists(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"))
            {
                _logger.Log("Chrome is not installed on the server.", LogLevel.Warning, "WebCommands");
            }
        }
        void CHeckFirefox()
        {
            if (!File.Exists(@"C:\Program Files (x86)\Mozilla Firefox\firefox.exe"))
            {
                _logger.Log("Firefox is not installed on the server.", LogLevel.Warning, "WebCommands");
            }
        }
        void CheckOpera()
        {
            if (!File.Exists(@"C:\Program Files\Opera\launcher.exe"))
            {
                _logger.Log("Opera is not installed on the server.", LogLevel.Warning, "WebCommands");
            }
        }

        [CommandHelp("Starts a new chrome seassion.")]
        public CommandResponse chrome(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                ServerManager.ServerRemoteService.RemoteInterface.RunProgram("cmd.exe", $"/c \"start chrome.exe {args.Arguments[1]}\"", false, false);
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
        public CommandResponse ie(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                ServerManager.ServerRemoteService.RemoteInterface.RunProgram("cmd.exe", $"/c \"start iexplore.exe {args.Arguments[1]}\"", false, false);
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
        public CommandResponse opera(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                ServerManager.ServerRemoteService.RemoteInterface.RunProgram("cmd.exe", $"/c \"start opera.exe {args.Arguments[1]}\"", false, false);
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
        public CommandResponse firefox(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                ServerManager.ServerRemoteService.RemoteInterface.RunProgram("cmd.exe", $"/c \"start firefox.exe {args.Arguments[1]}\"", false, false);
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