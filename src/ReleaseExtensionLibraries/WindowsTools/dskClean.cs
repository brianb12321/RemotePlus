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
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.RequestSystem;
using RemotePlusServer.Core;
using BetterLogger;

namespace WindowsTools
{
    internal static class dskClean
    {
        [CommandHelp("Cleans your disk of temperary files.")]
        public static CommandResponse dskCleanCommand(CommandRequest args, CommandPipeline pipe)
        {
            if (ServerManager.ServerRemoteService.RemoteInterface.Client.ClientType != RemotePlusLibrary.Client.ClientType.CommandLine)
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("dskClean does not work on a GUI yet.", BetterLogger.LogLevel.Error, "dskClean");
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
            var result = ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(optionBuilder);
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
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Operation aborted.", BetterLogger.LogLevel.Info, "dskClean");
                    break;
            }
            return new CommandResponse((int)CommandStatus.Success);
        }
        public static void cleanWindowsTempFolder()
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Cleaning Windows Temp Folder.", BetterLogger.LogLevel.Info, "dskClean");
            foreach (string file in Directory.GetFiles(@"C:\Windows\Temp", "*", SearchOption.AllDirectories))
            {
                try
                {
                    File.Delete(file);
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"File deleted: {file}", BetterLogger.LogLevel.Info, "dskClean");
                }
                catch (Exception ex)
                {
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Could not delete file {Path.GetFileName(file)}: {ex.Message}", BetterLogger.LogLevel.Warning, "dskClean");
                }
            }
        }
        public static void cleanAppDataTempFolder()
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Cleaning AppData Temp Folder.", BetterLogger.LogLevel.Info, "dskClean");
            foreach (string profile in Directory.GetDirectories(@"C:\Users"))
            {
                try
                {
                    foreach (string file in Directory.GetFiles($@"{profile}\AppData\Local\Temp", "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            File.Delete(file);
                            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"File deleted: {file}", BetterLogger.LogLevel.Info, "dskClean");
                        }
                        catch (Exception ex)
                        {
                            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Could not delete file {Path.GetFileName(file)}: {ex.Message}", BetterLogger.LogLevel.Warning, "dskClean");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Could not find directory {Path.GetFileName(profile)}: {ex.Message}", BetterLogger.LogLevel.Warning, "dskClean");
                }
            }
        }
        public static void cleanInternetExplorerCache()
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Cleaning Internet Explorer Cache.", BetterLogger.LogLevel.Info, "dskClean");
            foreach (string profile in Directory.GetDirectories(@"C:\Users"))
            {
                try
                {
                    foreach (string file in Directory.GetFiles($@"{profile}\AppData\Local\Microsoft\Windows\INetCache", "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            File.Delete(file);
                            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"File deleted: {file}", BetterLogger.LogLevel.Info, "dskClean");
                        }
                        catch (Exception ex)
                        {
                            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Could not delete file {Path.GetFileName(file)}: {ex.Message}", BetterLogger.LogLevel.Warning, "dskClean");
                        }
                    }
                }
                catch(Exception ex)
                {
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Could not find directory {Path.GetFileName(profile)}: {ex.Message}", BetterLogger.LogLevel.Warning, "dskClean");
                }
            }
        }
        [IronPython.Runtime.PythonHidden]
        public static void cleanRecycleBin()
        {
            try
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Cleaning Recycle Bin", BetterLogger.LogLevel.Info, "dskClean");
                foreach (string file in Directory.GetFiles(@"C:\$Recycle.Bin\Recycle Bin", "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        File.Delete(file);
                        ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"File deleted: {file}", BetterLogger.LogLevel.Info, "dskClean");
                    }
                    catch (Exception ex)
                    {
                        ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Could not delete file {Path.GetFileName(file)}: {ex.Message}", BetterLogger.LogLevel.Warning, "dskClean");
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Could not find Recycle Bin. Skipping clean.", BetterLogger.LogLevel.Warning, "dskClean");
            }
        }
        [IronPython.Runtime.PythonHidden]
        public static void ownAppDataTempFolder()
        {
            var result = (DialogResult)Enum.Parse(typeof(DialogResult), ((string)ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(RequestBuilder.RequestMessageBox($"WARNING: Continuing will override ownership of each user's AppData\\Temp Folder. All AppData\\Temp files will be owned by {Environment.UserName}. Do you want to proceed?", "File Ownership warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)).Data));
            if(result != DialogResult.Yes)
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Operation aborted.", BetterLogger.LogLevel.Info, "dskClean");
                return;
            }
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Owning AppData Temp Folder.", BetterLogger.LogLevel.Info, "dskClean");
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
                            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"File owned: {file}", BetterLogger.LogLevel.Info, "dskClean");
                        }
                        catch (Exception ex)
                        {
                            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Could not own file {Path.GetFileName(file)}: {ex.Message}", BetterLogger.LogLevel.Warning, "dskClean");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Could not find directory {Path.GetFileName(profile)}: {ex.Message}", BetterLogger.LogLevel.Warning, "dskClean");
                }
            }
        }
        [IronPython.Runtime.PythonHidden]
        public static void ownIECacheFolder()
        {
            var result = (DialogResult)Enum.Parse(typeof(DialogResult), ((string)ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(RequestBuilder.RequestMessageBox($"WARNING: Continuing will override ownership of each user's IE Cache Folder. All IE Cache files will be owned by {Environment.UserName}. Do you want to proceed?", "File Ownership warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)).Data));
            if (result != DialogResult.Yes)
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Operation aborted.", LogLevel.Info, "dskClean");
                return;
            }
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Owning IE Cache Folder.", LogLevel.Info, "dskClean");
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
                            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"File owned: {file}", LogLevel.Info, "dskClean");
                        }
                        catch (Exception ex)
                        {
                            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Could not own file {Path.GetFileName(file)}: {ex.Message}", LogLevel.Warning, "dskClean");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Could not find directory {Path.GetFileName(profile)}: {ex.Message}", LogLevel.Warning, "dskClean");
                }
            }
        }
        [IronPython.Runtime.PythonHidden]
        public static void ownWindowsTempFolder()
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Owning Windows Temp Folder.", LogLevel.Info, "dskClean");
            foreach (string file in Directory.GetFiles(@"C:\Windows\Temp", "*", SearchOption.AllDirectories))
            {
                try
                {
                    var fs = File.GetAccessControl(file);
                    fs.SetOwner(new NTAccount(Environment.UserDomainName, Environment.UserName));
                    File.SetAccessControl(file, fs);
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"File owned: {file}", LogLevel.Info, "dskClean");
                }
                catch (Exception ex)
                {
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Could not own file {Path.GetFileName(file)}: {ex.Message}", LogLevel.Warning, "dskClean");
                }
            }
        }
    }
    //Provides functions for the dskClean command
    /// <summary>
    /// Provides script functions for the dskClean command
    /// </summary>
    public class dskCleanScripting
    {
        /// <summary>
        /// Removes files from the Windows temp Folder.
        /// </summary>
        [IndexScriptObject]
        public void cleanWindowsTempFolder()
        {
            dskClean.cleanWindowsTempFolder();
        }
        /// <summary>
        /// Removes files from the AppData temp folder.
        /// </summary>
        [IndexScriptObject]
        public void cleanAppDataTempFolder()
        {
            dskClean.cleanAppDataTempFolder();
        }
        /// <summary>
        /// Removes files from the IE cache folder.
        /// </summary>
        [IndexScriptObject]
        public void cleanIECache()
        {
            dskClean.cleanInternetExplorerCache();
        }
        /// <summary>
        /// Takes ownership of all the files from the Windows temp folder to the current user.
        /// </summary>
        [IndexScriptObject]
        public void ownWindowsTempFolder()
        {
            dskClean.ownWindowsTempFolder();
        }
        /// <summary>
        /// Takes ownership of all the files from the AppData temp folder to the current user.
        /// </summary>
        [IndexScriptObject]
        public void ownAppDataTempFolder()
        {
            dskClean.ownAppDataTempFolder();
        }
        /// <summary>
        /// Takes ownership of all the files from the IE cache folder to the current user.
        /// </summary>
        [IndexScriptObject]
        public void ownIECache()
        {
            dskClean.ownIECacheFolder();
        }
    }
}