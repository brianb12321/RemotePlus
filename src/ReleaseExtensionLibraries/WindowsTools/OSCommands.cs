using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusServer;
using RemotePlusLibrary.Extension.CommandSystem;
using System.Windows.Forms;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using System.IO;

namespace WindowsTools
{
    public static class OSCommands
    {
        [CommandHelp("Sends a key to the remote server.")]
        public static CommandResponse sendKey(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                SendKeys.SendWait(args.Arguments[1].Value);
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, $"Key {args.Arguments[0]} sent to server."));
                return new CommandResponse((int)CommandStatus.Success);
            }   
            catch (Exception ex)
            {
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Error, $"Unable to send key {args.Arguments[0]} to the server. Error: {ex.Message}"));
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Open the disk drive on the remote computer.")]
        public static CommandResponse openDiskDrive(CommandRequest args, CommandPipeline pipe)
        {
            Win32Wrapper.OpenDiskDrive(args.Arguments[1].Value, "");
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Lists all the installed drives on the systme.")]
        public static CommandResponse drives(CommandRequest args, CommandPipeline pipe)
        {
            foreach(DriveInfo info in DriveInfo.GetDrives())
            {
                if (info.IsReady)
                {
                    ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole($"Name: {info.Name}\nSystem Type: {info.DriveFormat}\nDrive Type: {info.DriveType}\n\n");
                }
            }
            return new CommandResponse((int)CommandStatus.Success);
        }
    }
}