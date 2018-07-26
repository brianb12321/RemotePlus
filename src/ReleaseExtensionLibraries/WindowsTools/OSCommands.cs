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
using RemotePlusServer.Core;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Drawing;

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
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Key {args.Arguments[1]} sent to server.", BetterLogger.LogLevel.Info);
                return new CommandResponse((int)CommandStatus.Success);
            }   
            catch (Exception ex)
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Unable to send key {args.Arguments[1]} to the server. Error: {ex.Message}", BetterLogger.LogLevel.Error);
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
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Name: {info.Name}\nSystem Type: {info.DriveFormat}\nDrive Type: {info.DriveType}\n\n");
                }
            }
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Changes the mouse position to the specified coordinates")]
        public static CommandResponse setMousePos(CommandRequest args, CommandPipeline pipe)
        {
            Cursor.Position = new System.Drawing.Point(int.Parse(args.Arguments[1].Value), int.Parse(args.Arguments[2].Value));
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Blocks input for a certain amount of time.")]
        public static CommandResponse blockInputI(CommandRequest args, CommandPipeline pipe)
        {
            Win32Wrapper.BlockInputForInterval(int.Parse(args.Arguments[1].Value));
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Sets the server audio to a specific percentage.")]
        public static CommandResponse setVolume(CommandRequest args, CommandPipeline pipe)
        {
            if (args.Arguments.Count < 2)
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText("You must specify a percentage.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            else
            {
                if(int.TryParse(args.Arguments[1].Value, out int percent))
                {
                    CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
                    defaultPlaybackDevice.Volume = percent;
                    return new CommandResponse((int)CommandStatus.Success);
                }
                else
                {
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText("Given value is invalid.") { TextColor = Color.Red });
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
        }
    }
}