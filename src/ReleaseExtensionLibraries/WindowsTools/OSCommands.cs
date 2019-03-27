using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusServer;
using Microsoft.Win32;
using System.Windows.Forms;
using System.IO;
using RemotePlusServer.Core;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Drawing;
using NAudio.CoreAudioApi;
using BetterLogger;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.RequestSystem.DefaultUpdateRequestBuilders;
using NDesk.Options;
using Ninject;
using RemotePlusServer.Core.ExtensionSystem;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;

namespace WindowsTools
{
    public class OSCommands : ServerCommandClass
    {
        ILogFactory _logger;
        IRemotePlusService<ServerRemoteInterface> _service;
        [CommandHelp("Manages the registry on the system.")]
        public CommandResponse regEdit(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            string mode = string.Empty;
            string key = string.Empty;
            string value = string.Empty;
            string valueName = string.Empty;
            string valueType = "string";
            string hive = string.Empty;
            OptionSet set = new OptionSet()
                .Add("set", "Sets a value in the Registry", v => mode = "set")
                .Add("createKey", "Creates a key at a specified path.", v => mode = "createKey")
                .Add("deleteValue", "Deletes a value from a key.", v => mode = "delete")
                .Add("deleteKey", "Deletes a key from the Registry.", v => mode = "deleteKey")
                .Add("view", "View all values in a key.", v => mode = "view")
                .Add("key|k=", "The path to a key.", v => key = v)
                .Add("hive|h=", "The hive to open.", v => hive = v)
                .Add("value|v=", "The value of the value.", v => value = v)
                .Add("valueType|t=", "The type of value to add.", v => valueType = v)
                .Add("valueName|n=", "The name of the value.", v => valueName = v)
                .Add("help|?", "Shows the help screen.", v => mode = "help");
            set.Parse(args.Arguments.Select(a => a.ToString()));
            switch(mode)
            {
                case "help":
                    set.WriteOptionDescriptions(currentEnvironment.Out);
                    break;
                case "set":
                    RegistryKey regKey = openHive(hive).OpenSubKey(key, true);
                    RegistryValueKind kind = getKind(valueType);
                    regKey.SetValue(value, value, kind);
                    regKey.Dispose();
                    break;
                case "createKey":
                    openHive(hive).CreateSubKey(key).Dispose();
                    break;
                case "deleteValue":
                    openHive(hive).OpenSubKey(key, true).DeleteValue(valueName, true);
                    break;
                case "view":
                    RegistryKey openRegKey = openHive(hive).OpenSubKey(key, false);
                    string heading = $"Values for {key}";
                    currentEnvironment.WriteLine(heading);
                    currentEnvironment.WriteLine(new string('=', heading.Length));
                    foreach(var allValues in openRegKey.GetValueNames())
                    {
                        currentEnvironment.Write(allValues + ": ");
                        currentEnvironment.Write(openRegKey.GetValue(allValues).ToString());
                    }
                    break;
            }
            return new CommandResponse((int)CommandStatus.Success);
        }
        private RegistryKey openHive(string hive)
        {
            switch(hive.ToUpper())
            {
                case "LOCALMACHINE":
                    return Registry.LocalMachine;
                case "CURRENTUSER":
                    return Registry.CurrentUser;
                case "CURRENTCONFIG":
                    return Registry.CurrentConfig;
                case "CLASSESROOT":
                    return Registry.ClassesRoot;
                case "USERS":
                    return Registry.Users;
                case "PERFORMANCEDATA":
                    return Registry.PerformanceData;
                default:
                    throw new Exception("Hive does not exist.");
            }
        }
        private RegistryValueKind getKind(string valueType)
        {
            switch(valueType.ToUpper())
            {
                case "STRING":
                    return RegistryValueKind.String;
                case "MULTISTRING":
                    return RegistryValueKind.MultiString;
                case "EXPANDSTRING":
                    return RegistryValueKind.ExpandString;
                case "DWORD":
                    return RegistryValueKind.DWord;
                case "QWORD":
                    return RegistryValueKind.QWord;
                case "BINARY":
                    return RegistryValueKind.Binary;
                default:
                    throw new Exception("Kind not found.");
            }
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
        public override void InitializeServices(IKernel kernel)
        {
            _service = kernel.Get<IRemotePlusService<ServerRemoteInterface>>();
            _logger = kernel.Get<ILogFactory>();
            _logger.Log("Adding OS commands", LogLevel.Info, "WindowsTools");
            Commands.Add("regEdit", regEdit);
            Commands.Add("fileM", Filem.filem_command);
            Commands.Add("openDiskDrive", openDiskDrive);
            Commands.Add("drives", drives);
            Commands.Add("setMousePos", setMousePos);
            Commands.Add("blockInputI", blockInputI);
            Commands.Add("toggleMute", toggleMute);
            Commands.Add("sendKey", sendKey);
            Commands.Add("hugeFile", hugeFile);
            _logger.Log("Adding dskClean command", LogLevel.Info, "WindowsTools");
            Commands.Add("dskClean", dskClean.dskCleanCommand);
            _logger.Log("Adding fileM command", LogLevel.Info, "WindowsTools");
        }
    }
}