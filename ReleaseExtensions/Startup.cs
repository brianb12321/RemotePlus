using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension;
using RemotePlusServer;
using RemotePlusLibrary.Extension.CommandSystem;
using System.IO;
using System.Reflection;
using RemotePlusLibrary.Extension.ClientCommandSystem;
using RemotePlusLibrary.Extension.ExtensionLibraries.LibraryStartupTypes;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;

namespace ReleaseExtensions
{
    public sealed class Startup : ILibraryStartup, IClientCommandLibraryStartup, IClientLibraryStartup
    {
        void IClientLibraryStartup.ClientInit(ILibraryBuilder builder, IInitEnvironment env)
        {
            RemotePlusClient.MainF.ConsoleObj.Logger.AddOutput($"Client position {env.InitPosition}", Logging.OutputLevel.Debug, "ReleaseExtensions");
        }
        void ILibraryStartup.Init(ILibraryBuilder builder, IInitEnvironment env)
        {
            ServerManager.Logger.AddOutput($"Init position {env.InitPosition}", Logging.OutputLevel.Debug, "ReleaseExtensions");
            ServerManager.Logger.AddOutput(new Logging.LogItem(Logging.OutputLevel.Info, "Welcome to \"ReleaseExtension.\" This library contains some useful tools that demonstrates the powers of \"RemotePlus\"", "ReleaseExtensions") { Color = Console.ForegroundColor});
            ServerManager.DefaultService.Commands.Add("releaseExtensionAbout", releaseExtensionAbout);
            ServerManager.Logger.AddOutput(new Logging.LogItem(Logging.OutputLevel.Info, "Adding watchers.", "ReleaseExtensions") { Color = Console.ForegroundColor });
            ServerManager.Watchers.Add("HddWatcher", new HddWatcher());
            ServerManager.Watchers.Add("SerialWatcher", new SerialWatcher());
        }

        void IClientCommandLibraryStartup.ModuleLibraryInit()
        {
            RemotePlusClientCmd.ClientCmdManager.LocalCommands.Add("#sayHello", client_sayHello);
        }

        [CommandHelp("Describes about the ReleaseExtensionsLibrary.")]
        CommandResponse releaseExtensionAbout(CommandRequest args, CommandPipeline pipe)
        {
            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "ReleaseExtension is a test of the extension system."));
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Say hello")]
        CommandResponse client_sayHello(CommandRequest args, CommandPipeline pipe)
        {
            RemotePlusClientCmd.ClientCmdManager.Logger.AddOutput("Hello", Logging.OutputLevel.Info);
            return new CommandResponse((int)CommandStatus.Success);
        }
    }
}