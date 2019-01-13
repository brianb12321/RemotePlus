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
using NAudio.CoreAudioApi;
using BetterLogger;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.RequestSystem.DefaultUpdateRequestBuilders;

namespace WindowsTools
{
    public class OSCommands : StandordCommandClass
    {
        ILogFactory _logger;
        IRemotePlusService<ServerRemoteInterface> _service;
        public OSCommands(ILogFactory logger, IRemotePlusService<ServerRemoteInterface> service)
        {
            _service = service;
            _logger = logger;
        }
        [CommandHelp("Sends a key to the remote server.")]
        public CommandResponse sendKey(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            try
            {
                SendKeys.SendWait(args.Arguments[1].ToString());
                currentEnvironment.WriteLine($"Key {args.Arguments[1]} sent to server.");
                return new CommandResponse((int)CommandStatus.Success);
            }   
            catch (Exception ex)
            {
                currentEnvironment.WriteLine(new ConsoleText($"Unable to send key {args.Arguments[1]} to the server. Error: {ex.Message}") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Open the disk drive on the remote computer.")]
        public CommandResponse openDiskDrive(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            Win32Wrapper.OpenDiskDrive(args.Arguments[1].ToString(), "");
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Lists all the installed drives on the system.")]
        public CommandResponse drives(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            foreach(DriveInfo info in DriveInfo.GetDrives())
            {
                if (info.IsReady)
                {
                    currentEnvironment.WriteLine($"Name: {info.Name}\nSystem Type: {info.DriveFormat}\nDrive Type: {info.DriveType}\n\n");
                }
            }
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Changes the mouse position to the specified coordinates")]
        public CommandResponse setMousePos(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            Cursor.Position = new System.Drawing.Point(int.Parse(args.Arguments[1].ToString()), int.Parse(args.Arguments[2].ToString()));
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Blocks input for a certain amount of time.")]
        public CommandResponse blockInputI(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            Win32Wrapper.BlockInputForInterval(int.Parse(args.Arguments[1].ToString()));
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Sets the server audio to a specific percentage.")]
        public CommandResponse setVolume(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            if (args.Arguments.Count < 2)
            {
                currentEnvironment.WriteLine(new ConsoleText("You must specify a percentage.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            else
            {
                if(int.TryParse(args.Arguments[1].ToString(), out int percent))
                {
                    CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
                    defaultPlaybackDevice.Volume = percent;
                    return new CommandResponse((int)CommandStatus.Success);
                }
                else
                {
                    currentEnvironment.WriteLine(new ConsoleText("Given ToString() is invalid.") { TextColor = Color.Red });
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
        }
        [CommandHelp("Toggles the mute on the server.")]
        public CommandResponse toggleMute(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            foreach(MMDevice device in enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All))
            {
                try
                {
                    device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
                }
                catch
                {

                }
            }
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Writes specified amount of random data to disk.")]
        public CommandResponse hugeFile(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            StreamWriter sw = null;
            try
            {
                Random r = new Random();
                sw = new StreamWriter(args.Arguments[1].ToString());
                int max = int.Parse(args.Arguments[2].ToString());
                _service.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Beginning write operation.");
                _service.RemoteInterface.Client.ClientCallback.RequestInformation(new ProgressRequestBuilder()
                {
                    Message = "Writing to disk.",
                    Maximum = max
                });
                for (int i = 0; i <= max; i++)
                {
                    sw.WriteLine(r.Next(0, int.MaxValue));
                    _service.RemoteInterface.Client.ClientCallback.UpdateRequest(new ProgressUpdateBuilder(i)
                    {
                        Text = $"{i} / {max} written."
                    });
                }
                _service.RemoteInterface.Client.ClientCallback.DisposeCurrentRequest();
                return new CommandResponse((int)CommandStatus.Success);
            }
            finally
            {
                if(sw != null) sw.Dispose();
            }
        }
        public override void AddCommands()
        {
            _logger.Log("Adding OS commands", LogLevel.Info, "WindowsTools");
            Commands.Add("fileM", Filem.filem_command);
            Commands.Add("openDiskDrive", openDiskDrive);
            Commands.Add("drives", drives);
            Commands.Add("setMousePos", setMousePos);
            Commands.Add("blockInputI", blockInputI);
            Commands.Add("setVolume", setVolume);
            Commands.Add("toggleMute", toggleMute);
            Commands.Add("sendKey", sendKey);
            Commands.Add("hugeFile", hugeFile);
            _logger.Log("Adding dskClean command", LogLevel.Info, "WindowsTools");
            Commands.Add("dskClean", dskClean.dskCleanCommand);
            _logger.Log("Adding fileM command", LogLevel.Info, "WindowsTools");
        }
    }
}