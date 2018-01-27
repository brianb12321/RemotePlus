using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsTools
{
    public class Startup : ILibraryStartup
    {
        public void Init(ILibraryBuilder builder, IInitEnvironment env)
        {
            ServerManager.Logger.AddOutput($"Init position {env.InitPosition}", Logging.OutputLevel.Debug, "WindowsTools");
            ServerManager.Logger.AddOutput("Adding OS commands", Logging.OutputLevel.Info, "WindowsTools");
            ServerManager.DefaultService.Commands.Add("sendKey", OSCommands.sendKey);
            ServerManager.Logger.AddOutput("Adding dskClean command", Logging.OutputLevel.Info, "WindowsTools");
            ServerManager.DefaultService.Commands.Add("dskClean", dskClean.dskCleanCommand);
            ServerManager.Logger.AddOutput("Adding filem command", Logging.OutputLevel.Info, "WindowsTools");
            ServerManager.DefaultService.Commands.Add("fileM", Filem.filem_command);
            //RemotePlusServer.ScriptingEngine.ScriptBuilder.AddFunction<Action>("runFileM", () => ServerManager.DefaultService.Remote.RunServerCommand("fileM", CommandExecutionMode.Script));
        }
    }
}
