using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using RemotePlusServer.Core;
using BetterLogger;

namespace WindowsTools
{
    public class Startup : ILibraryStartup
    {
        public void Init(ILibraryBuilder builder, IInitEnvironment env)
        {
            ServerManager.Logger.Log($"Init position {env.InitPosition}", LogLevel.Debug, "WindowsTools");
            ServerManager.Logger.Log("Adding OS commands", LogLevel.Info, "WindowsTools");
            ServerManager.ServerRemoteService.Commands.Add("sendKey", OSCommands.sendKey);
            ServerManager.Logger.Log("Adding dskClean command", LogLevel.Info, "WindowsTools");
            ServerManager.ServerRemoteService.Commands.Add("dskClean", dskClean.dskCleanCommand);
            ServerManager.Logger.Log("Adding filem command", LogLevel.Info, "WindowsTools");
            ServerManager.ServerRemoteService.Commands.Add("fileM", Filem.filem_command);
            ServerManager.ServerRemoteService.Commands.Add("openDiskDrive", OSCommands.openDiskDrive);
            ServerManager.ServerRemoteService.Commands.Add("drives", OSCommands.drives);
            ServerManager.ServerRemoteService.Commands.Add("setMousePos", OSCommands.setMousePos);
            ServerManager.ServerRemoteService.Commands.Add("blockInputI", OSCommands.blockInputI);
            //RemotePlusServer.ScriptingEngine.ScriptBuilder.AddFunction<Action>("runFileM", () => ServerManager.ServerRemoteService.RemoteInterface.RunServerCommand("fileM", CommandExecutionMode.Script));
        }
    }
}
