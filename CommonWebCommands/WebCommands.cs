using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusServer;

namespace CommonWebCommands
{
    public static class WebCommands
    {
        [CommandHelp("Starts a new chrome seassion.")]
        public static int chrome(string[] args)
        {
            try
            {
                ServerManager.DefaultService.Remote.RunProgram("cmd.exe", $"/c \"start chrome.exe {args[1]}\"");
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "chrome started", "WebCommands"));
                return (int)CommandStatus.Success;
            }
            catch
            {
                return (int)CommandStatus.Fail;
                throw;
            }
        }
        [CommandHelp("Starts a new internet explorer seassion.")]
        public static int ie(string[] args)
        {
            try
            {
                ServerManager.DefaultService.Remote.RunProgram("cmd.exe", $"/c \"start iexplore.exe {args[1]}\"");
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "chrome started", "WebCommands"));
                return (int)CommandStatus.Success;
            }
            catch
            {
                return (int)CommandStatus.Fail;
                throw;
            }
        }
        [CommandHelp("Starts a new Opera seassion")]
        public static int opera(string[] args)
        {
            try
            {
                ServerManager.DefaultService.Remote.RunProgram("cmd.exe", $"/c \"start opera.exe {args[1]}\"");
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "Opera started", "WebCommands"));
                return (int)CommandStatus.Success;
            }
            catch
            {
                return (int)CommandStatus.Fail;
                throw;
            }
        }
        [CommandHelp("Starts a new Firefox seassion")]
        public static int firefox(string[] args)
        {
            try
            {
                ServerManager.DefaultService.Remote.RunProgram("cmd.exe", $"/c \"start firefox.exe {args[1]}\"");
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "Firefox started", "WebCommands"));
                return (int)CommandStatus.Success;
            }
            catch
            {
                return (int)CommandStatus.Fail;
                throw;
            }
        }
    }
}
