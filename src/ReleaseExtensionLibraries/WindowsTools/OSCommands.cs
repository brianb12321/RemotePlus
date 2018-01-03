using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusServer;
using RemotePlusLibrary.Extension.CommandSystem;
using System.Windows.Forms;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;

namespace WindowsTools
{
    public static class OSCommands
    {
        [CommandHelp("Sends a key to the remote server.")]
        public static CommandResponse sendKey(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                SendKeys.SendWait(args.Arguments[0].Value);
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, $"Key {args.Arguments[0]} sent to server."));
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (Exception ex)
            {
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Error, $"Unable to send key {args.Arguments[0]} to the server. Error: {ex.Message}"));
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
    }
}
