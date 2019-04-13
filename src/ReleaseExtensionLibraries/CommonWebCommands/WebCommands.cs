using RemotePlusServer.Core;
using System.IO;
using BetterLogger;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension;
using RemotePlusServer.Core.ExtensionSystem;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.SubSystem.Command;

namespace CommonWebCommands
{
    [ExtensionModule]
    public class WebCommands : ServerCommandClass
    {
        private ILogFactory _logger;
        private IRemotePlusService<ServerRemoteInterface> _service;
        public override void InitializeServices(IServiceCollection services)
        {
            _logger = services.GetService<ILogFactory>();
            _service = services.GetService<IRemotePlusService<ServerRemoteInterface>>();
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

        [CommandHelp("Starts a new chrome session.")]
        public CommandResponse chrome(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            try
            {
                _service.RemoteInterface.RunProgram("cmd.exe", $"/c \"start chrome.exe {args.Arguments[1]}\"", false, false);
                currentEnvironment.WriteLine("Chrome started");
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch
            {
                return new CommandResponse((int)CommandStatus.Fail);
                throw;
            }
        }
        [CommandHelp("Starts a new internet explorer session.")]
        public CommandResponse ie(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            try
            {
                _service.RemoteInterface.RunProgram("cmd.exe", $"/c \"start iexplore.exe {args.Arguments[1]}\"", false, false);
                currentEnvironment.WriteLine("Chrome started");
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch
            {
                return new CommandResponse((int)CommandStatus.Fail);
                throw;
            }
        }
        [CommandHelp("Starts a new Opera session")]
        public CommandResponse opera(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            try
            {
                _service.RemoteInterface.RunProgram("cmd.exe", $"/c \"start opera.exe {args.Arguments[1]}\"", false, false);
                currentEnvironment.WriteLine("Opera started");
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch
            {
                return new CommandResponse((int)CommandStatus.Fail);
                throw;
            }
        }
        [CommandHelp("Starts a new Firefox session")]
        public CommandResponse firefox(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            try
            {
                _service.RemoteInterface.RunProgram("cmd.exe", $"/c \"start firefox.exe {args.Arguments[1]}\"", false, false);
                currentEnvironment.WriteLine("Firefox started");
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