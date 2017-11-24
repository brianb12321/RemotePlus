using RemotePlusLibrary;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;

namespace WindowsTools
{
    public static class dskClean
    {
        [CommandHelp("Cleans your disk of temperary files.")]
        public static CommandResponse dskCleanCommand(CommandRequest args, CommandPipeline pipe)
        {
            if (ServerManager.DefaultService.Remote.Client.ClientType != RemotePlusLibrary.ClientType.CommandLine)
            {
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Error, "dskClean does not work on a GUI yet.", "dskClean"));
                return new CommandResponse(-3);
            }
            Dictionary<string, string> cleanOptions = new Dictionary<string, string>();
            cleanOptions.Add("0", "Windows Temp Folder.");
            cleanOptions.Add("1", "AppData Temp Folder.");
            cleanOptions.Add("2", "Internet Explorer Cache");
            cleanOptions.Add("3", "Recycle Bin");
            cleanOptions.Add("4", "Take own of all AppData Temp folder");
            cleanOptions.Add("5", "Take own of all Internet Explorer Cache Folder");
            cleanOptions.Add("6", "Take own of all Windows Temp Folder");
            cleanOptions.Add("7", "All Temp Folders");
            cleanOptions.Add("8", "Clean all temp folders and take own");
            cleanOptions.Add("9", "Exit");
            var optionBuilder = new RequestBuilder("rcmd_smenu", "What should dskClean clean from the server?\nNOTE: These actions will be performed on all users except for the Recycle Bin clean.", cleanOptions);
            var result = ServerManager.DefaultService.Remote.Client.ClientCallback.RequestInformation(optionBuilder);
            switch((char)result.Data)
            {
                case '0':
                    cleanWindowsTempFolder();
                    break;
                case '1':
                    cleanAppDataTempFolder();
                    break;
                case '2':
                    cleanInternetExplorerCache();
                    break;
                case '3':
                    cleanRecycleBin();
                    break;
                case '4':
                    ownAppDataTempFolder();
                    break;
                case '5':
                    ownIECacheFolder();
                    break;
                case '6':
                    ownWindowsTempFolder();
                    break;
                case '7':
                    cleanWindowsTempFolder();
                    cleanAppDataTempFolder();
                    cleanInternetExplorerCache();
                    cleanRecycleBin();
                    break;
                case '8':
                    ownAppDataTempFolder();
                    ownIECacheFolder();
                    ownWindowsTempFolder();
                    cleanWindowsTempFolder();
                    cleanAppDataTempFolder();
                    cleanInternetExplorerCache();
                    cleanRecycleBin();
                    break;
                case '9':
                    ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "Operation aborted.", "dskClean"));
                    break;
            }
            return new CommandResponse((int)CommandStatus.Success);
        }
        static void cleanWindowsTempFolder()
        {
            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "Cleaning Windows Temp Folder.", "dskClean"));
            foreach (string file in Directory.GetFiles(@"C:\Windows\Temp", "*", SearchOption.AllDirectories))
            {
                try
                {
                    File.Delete(file);
                    ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, $"File deleted: {file}", "dskClean"));
                }
                catch (Exception ex)
                {
                    ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Warning, $"Could not delete file {Path.GetFileName(file)}: {ex.Message}", "dskClean"));
                }
            }
        }
        static void cleanAppDataTempFolder()
        {
            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "Cleaning AppData Temp Folder.", "dskClean"));
            foreach (string profile in Directory.GetDirectories(@"C:\Users"))
            {
                try
                {
                    foreach (string file in Directory.GetFiles($@"{profile}\AppData\Local\Temp", "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            File.Delete(file);
                            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, $"File deleted: {file}", "dskClean"));
                        }
                        catch (Exception ex)
                        {
                            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Warning, $"Could not delete file {Path.GetFileName(file)}: {ex.Message}", "dskClean"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Warning, $"Could not find directory {Path.GetFileName(profile)}: {ex.Message}", "dskClean"));
                }
            }
        }
        static void cleanInternetExplorerCache()
        {
            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "Cleaning Internet Explorer Cache.", "dskClean"));
            foreach (string profile in Directory.GetDirectories(@"C:\Users"))
            {
                try
                {
                    foreach (string file in Directory.GetFiles($@"{profile}\AppData\Local\Microsoft\Windows\INetCache", "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            File.Delete(file);
                            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, $"File deleted: {file}", "dskClean"));
                        }
                        catch (Exception ex)
                        {
                            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Warning, $"Could not delete file {Path.GetFileName(file)}: {ex.Message}", "dskClean"));
                        }
                    }
                }
                catch(Exception ex)
                {
                    ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Warning, $"Could not find directory {Path.GetFileName(profile)}: {ex.Message}", "dskClean"));
                }
            }
        }
        static void cleanRecycleBin()
        {
            try
            {
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "Cleaning Recycle Bin", "dskClean"));
                foreach (string file in Directory.GetFiles(@"C:\$Recycle.Bin\Recycle Bin", "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        File.Delete(file);
                        ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, $"File deleted: {file}", "dskClean"));
                    }
                    catch (Exception ex)
                    {
                        ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Warning, $"Could not delete file {Path.GetFileName(file)}: {ex.Message}", "dskClean"));
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Warning, $"Could not find Recycle Bin. Skipping clean.", "dskClean"));
            }
        }
        static void ownAppDataTempFolder()
        {
            var result = (DialogResult)Enum.Parse(typeof(DialogResult), ((string)ServerManager.DefaultService.Remote.Client.ClientCallback.RequestInformation(RequestBuilder.RequestMessageBox($"WARNING: Continuing will override ownership of each user's AppData\\Temp Folder. All AppData\\Temp files will be owned by {Environment.UserName}. Do you want to proceed?", "File Ownership warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)).Data));
            if(result != DialogResult.Yes)
            {
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "Operation aborted.", "dskClean"));
                return;
            }
            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "Owning AppData Temp Folder.", "dskClean"));
            foreach (string profile in Directory.GetDirectories(@"C:\Users"))
            {
                try
                {
                    foreach (string file in Directory.GetFiles($@"{profile}\AppData\Local\Temp", "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            var fs = File.GetAccessControl(file);
                            fs.SetOwner(new NTAccount(Environment.UserDomainName, Environment.UserName));
                            File.SetAccessControl(file, fs);
                            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, $"File owned: {file}", "dskClean"));
                        }
                        catch (Exception ex)
                        {
                            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Warning, $"Could not own file {Path.GetFileName(file)}: {ex.Message}", "dskClean"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Warning, $"Could not find directory {Path.GetFileName(profile)}: {ex.Message}", "dskClean"));
                }
            }
        }
        static void ownIECacheFolder()
        {
            var result = (DialogResult)Enum.Parse(typeof(DialogResult), ((string)ServerManager.DefaultService.Remote.Client.ClientCallback.RequestInformation(RequestBuilder.RequestMessageBox($"WARNING: Continuing will override ownership of each user's IE Cache Folder. All IE Cache files will be owned by {Environment.UserName}. Do you want to proceed?", "File Ownership warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)).Data));
            if (result != DialogResult.Yes)
            {
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "Operation aborted.", "dskClean"));
                return;
            }
            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "Owning IE Cache Folder.", "dskClean"));
            foreach (string profile in Directory.GetDirectories(@"C:\Users"))
            {
                try
                {
                    foreach (string file in Directory.GetFiles($@"{profile}\AppData\Local\Microsoft\Windows\INetCache", "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            var fs = File.GetAccessControl(file);
                            fs.SetOwner(new NTAccount(Environment.UserDomainName, Environment.UserName));
                            File.SetAccessControl(file, fs);
                            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, $"File owned: {file}", "dskClean"));
                        }
                        catch (Exception ex)
                        {
                            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Warning, $"Could not own file {Path.GetFileName(file)}: {ex.Message}", "dskClean"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Warning, $"Could not find directory {Path.GetFileName(profile)}: {ex.Message}", "dskClean"));
                }
            }
        }
        static void ownWindowsTempFolder()
        {
            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "Owning Windows Temp Folder.", "dskClean"));
            foreach (string file in Directory.GetFiles(@"C:\Windows\Temp", "*", SearchOption.AllDirectories))
            {
                try
                {
                    var fs = File.GetAccessControl(file);
                    fs.SetOwner(new NTAccount(Environment.UserDomainName, Environment.UserName));
                    File.SetAccessControl(file, fs);
                    ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, $"File owned: {file}", "dskClean"));
                }
                catch (Exception ex)
                {
                    ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Warning, $"Could not own file {Path.GetFileName(file)}: {ex.Message}", "dskClean"));
                }
            }
        }
    }
}