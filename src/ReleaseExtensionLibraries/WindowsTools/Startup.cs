using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using RemotePlusServer.Core;
using BetterLogger;
using RemotePlusLibrary;

namespace WindowsTools
{
    public class Startup : ILibraryStartup
    {
        public void Init(ILibraryBuilder builder, IInitEnvironment env)
        {
            GlobalServices.Logger.Log($"Init position {env.InitPosition}", LogLevel.Debug, "WindowsTools");
            GlobalServices.Logger.Log("Adding OS commands", LogLevel.Info, "WindowsTools");
            ServerManager.ServerRemoteService.Commands.Add("sendKey", OSCommands.sendKey);
            GlobalServices.Logger.Log("Adding dskClean command", LogLevel.Info, "WindowsTools");
            ServerManager.ServerRemoteService.Commands.Add("dskClean", dskClean.dskCleanCommand);
            GlobalServices.Logger.Log("Adding filem command", LogLevel.Info, "WindowsTools");
            ServerManager.ServerRemoteService.Commands.Add("fileM", Filem.filem_command);
            ServerManager.ServerRemoteService.Commands.Add("openDiskDrive", OSCommands.openDiskDrive);
            ServerManager.ServerRemoteService.Commands.Add("drives", OSCommands.drives);
            ServerManager.ServerRemoteService.Commands.Add("setMousePos", OSCommands.setMousePos);
            ServerManager.ServerRemoteService.Commands.Add("blockInputI", OSCommands.blockInputI);
            //RemotePlusServer.ScriptingEngine.ScriptBuilder.AddFunction<Action>("runFileM", () => ServerManager.ServerRemoteService.RemoteInterface.RunServerCommand("fileM", CommandExecutionMode.Script));
        }
    }
}
