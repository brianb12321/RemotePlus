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

namespace WindowsTools
{
    public class Startup : ILibraryStartup
    {
        public void Init(ILibraryBuilder builder, IInitEnvironment env)
        {
            ServerManager.Logger.AddOutput($"Init position {env.InitPosition}", Logging.OutputLevel.Debug, "WindowsTools");
            ServerManager.Logger.AddOutput("Adding OS commands", Logging.OutputLevel.Info, "WindowsTools");
            ServerManager.ServerRemoteService.Commands.Add("sendKey", OSCommands.sendKey);
            ServerManager.Logger.AddOutput("Adding dskClean command", Logging.OutputLevel.Info, "WindowsTools");
            ServerManager.ServerRemoteService.Commands.Add("dskClean", dskClean.dskCleanCommand);
            ServerManager.Logger.AddOutput("Adding filem command", Logging.OutputLevel.Info, "WindowsTools");
            ServerManager.ServerRemoteService.Commands.Add("fileM", Filem.filem_command);
            ServerManager.ServerRemoteService.Commands.Add("openDiskDrive", OSCommands.openDiskDrive);
            ServerManager.ServerRemoteService.Commands.Add("drives", OSCommands.drives);
            ServerManager.ServerRemoteService.Commands.Add("setMousePos", OSCommands.setMousePos);
            ServerManager.ServerRemoteService.Commands.Add("blockInputI", OSCommands.blockInputI);
            //RemotePlusServer.ScriptingEngine.ScriptBuilder.AddFunction<Action>("runFileM", () => ServerManager.ServerRemoteService.RemoteInterface.RunServerCommand("fileM", CommandExecutionMode.Script));
        }
    }
}
