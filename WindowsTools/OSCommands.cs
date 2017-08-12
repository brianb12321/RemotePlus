using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusServer;
using RemotePlusLibrary.Extension.CommandSystem;
using System.Windows.Forms;

namespace WindowsTools
{
    public static class OSCommands
    {
        [CommandHelp("Sends a key to the remote server.")]
        public static int sendKey(string[] args)
        {
            try
            {
                SendKeys.SendWait(args[0]);
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, $"Key {args[0]} sent to server."));
                return (int)CommandStatus.Success;
            }
            catch (Exception ex)
            {
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Error, $"Unable to send key {args[0]} to the server. Error: {ex.Message}"));
                return (int)CommandStatus.Fail;
            }
        }
    }
}
