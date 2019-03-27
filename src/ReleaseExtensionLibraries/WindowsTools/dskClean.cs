using RemotePlusLibrary;
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
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using System.Drawing;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.Core;

namespace WindowsTools
{
    internal static class dskClean
    {
        private static ICommandEnvironment currentEnvironment;
        [CommandHelp("Cleans your disk of temperary files.")]
        public static CommandResponse dskCleanCommand(CommandRequest args, CommandPipeline pipe, ICommandEnvironment _currentEnvironment)
        {
            currentEnvironment = _currentEnvironment;
            if (ServerManager.ServerRemoteService.RemoteInterface.Client.ClientType != ClientType.CommandLine)
            {
                currentEnvironment.WriteLine("dskClean does not work on a GUI yet.");
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
            var rb = new SMenuRequestBuilder()
            {
                MenuItems = cleanOptions,
                Message = "What should dskClean clean from the server?\nNOTE: These actions will be performed on all users except for the Recycle Bin clean."
            };
            var result = ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(rb);
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
                    currentEnvironment.WriteLine("Operation aborted.");
                    break;
            }
            return new CommandResponse((int)CommandStatus.Success);
        }
        public static void cleanWindowsTempFolder()
        {
            currentEnvironment.WriteLine("Cleaning Windows Temp Folder.");
            foreach (string file in Directory.GetFiles(@"C:\Windows\Temp", "*", SearchOption.AllDirectories))
            {
                try
                {
                    File.Delete(file);
                    currentEnvironment.WriteLine($"File deleted: {file}");
                }
                catch (Exception ex)
                {
                    currentEnvironment.WriteLine($"Could not delete file {Path.GetFileName(file)}: {ex.Message}", Color.Yellow);
                }
            }
        }
        public static void cleanAppDataTempFolder()
        {
            currentEnvironment.WriteLine("Cleaning AppData Temp Folder.");
            foreach (string profile in Directory.GetDirectories(@"C:\Users"))
            {
                try
                {
                    foreach (string file in Directory.GetFiles($@"{profile}\AppData\Local\Temp", "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            File.Delete(file);
                            currentEnvironment.WriteLine($"File deleted: {file}");
                        }
                        catch (Exception ex)
                        {
                            currentEnvironment.WriteLine($"Could not delete file {Path.GetFileName(file)}: {ex.Message}", Color.Yellow);
                        }
                    }
                }
                catch (Exception ex)
                {
                    currentEnvironment.WriteLine($"Could access directory {Path.GetFileName(profile)}: {ex.Message}", Color.Yellow);
                }
            }
        }
        public static void cleanInternetExplorerCache()
        {
            currentEnvironment.WriteLine("Cleaning Internet Explorer Cache.");
            foreach (string profile in Directory.GetDirectories(@"C:\Users"))
            {
                try
                {
                    foreach (string file in Directory.GetFiles($@"{profile}\AppData\Local\Microsoft\Windows\INetCache", "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            File.Delete(file);
                            currentEnvironment.WriteLine($"File deleted: {file}");
                        }
                        catch (Exception ex)
                        {
                            currentEnvironment.WriteLine($"Could not delete file {Path.GetFileName(file)}: {ex.Message}", Color.Yellow);
                        }
                    }
                }
                catch(Exception ex)
                {
                    currentEnvironment.WriteLine($"Could not access directory {Path.GetFileName(profile)}: {ex.Message}", Color.Yellow);
                }
            }
        }
        [IronPython.Runtime.PythonHidden]
        public static void cleanRecycleBin()
        {
            try
            {
                currentEnvironment.WriteLine("Cleaning Recycle Bin");
                foreach (string file in Directory.GetFiles(@"C:\$Recycle.Bin\Recycle Bin", "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        File.Delete(file);
                        currentEnvironment.WriteLine($"File deleted: {file}");
                    }
                    catch (Exception ex)
                    {
                        currentEnvironment.WriteLine($"Could not delete file {Path.GetFileName(file)}: {ex.Message}", Color.Yellow);
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                currentEnvironment.WriteLine($"Could not find Recycle Bin. Skipping clean.");
            }
        }
        [IronPython.Runtime.PythonHidden]
        public static void ownAppDataTempFolder()
        {
            var rb = new MessageBoxRequestBuilder()
            {
                Message = $"WARNING: Continuing will override ownership of each user's AppData\\Temp Folder. All AppData\\Temp files will be owned by {Environment.UserName}. Do you want to proceed?",
                Caption = "File Ownership warning",
                Buttons = MessageBoxButtons.YesNo,
                Icons = MessageBoxIcon.Warning
            };
            var result = (DialogResult)Enum.Parse(typeof(DialogResult), ((string)ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(rb).Data));
            if(result != DialogResult.Yes)
            {
                currentEnvironment.WriteLine("Operation aborted.");
                return;
            }
            currentEnvironment.WriteLine("Owning AppData Temp Folder.");
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
                            currentEnvironment.WriteLine($"File owned: {file}");
                        }
                        catch (Exception ex)
                        {
                            currentEnvironment.WriteLine($"Could not own file {Path.GetFileName(file)}: {ex.Message}", Color.Yellow);
                        }
                    }
                }
                catch (Exception ex)
                {
                    currentEnvironment.WriteLine($"Could not access directory {Path.GetFileName(profile)}: {ex.Message}", Color.Yellow);
                }
            }
        }
        [IronPython.Runtime.PythonHidden]
        public static void ownIECacheFolder()
        {
            var rb = new MessageBoxRequestBuilder()
            {
                Message = $"WARNING: Continuing will override ownership of each user's IE Cache Folder. All IE Cache files will be owned by {Environment.UserName}. Do you want to proceed?",
                Caption = "File Ownership warning",
                Buttons = MessageBoxButtons.YesNo,
                Icons = MessageBoxIcon.Warning
            };
            var result = (DialogResult)Enum.Parse(typeof(DialogResult), ((string)ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(rb).Data));
            if (result != DialogResult.Yes)
            {
                currentEnvironment.WriteLine("Operation aborted.");
                return;
            }
            currentEnvironment.WriteLine("Owning IE Cache Folder.");
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
                            currentEnvironment.WriteLine($"File owned: {file}");
                        }
                        catch (Exception ex)
                        {
                            currentEnvironment.WriteLine($"Could not own file {Path.GetFileName(file)}: {ex.Message}", Color.Yellow);
                        }
                    }
                }
                catch (Exception ex)
                {
                    currentEnvironment.WriteLine($"Could not access directory {Path.GetFileName(profile)}: {ex.Message}", Color.Yellow);
                }
            }
        }
        [IronPython.Runtime.PythonHidden]
        public static void ownWindowsTempFolder()
        {
            currentEnvironment.WriteLine("Owning Windows Temp Folder.");
            foreach (string file in Directory.GetFiles(@"C:\Windows\Temp", "*", SearchOption.AllDirectories))
            {
                try
                {
                    var fs = File.GetAccessControl(file);
                    fs.SetOwner(new NTAccount(Environment.UserDomainName, Environment.UserName));
                    File.SetAccessControl(file, fs);
                    currentEnvironment.WriteLine($"File owned: {file}");
                }
                catch (Exception ex)
                {
                    currentEnvironment.WriteLine($"Could not own file {Path.GetFileName(file)}: {ex.Message}", Color.Yellow);
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
        public void cleanWindowsTempFolder()
        {
            dskClean.cleanWindowsTempFolder();
        }
        /// <summary>
        /// Removes files from the AppData temp folder.
        /// </summary>
        public void cleanAppDataTempFolder()
        {
            dskClean.cleanAppDataTempFolder();
        }
        /// <summary>
        /// Removes files from the IE cache folder.
        /// </summary>
        public void cleanIECache()
        {
            dskClean.cleanInternetExplorerCache();
        }
        /// <summary>
        /// Takes ownership of all the files from the Windows temp folder to the current user.
        /// </summary>
        public void ownWindowsTempFolder()
        {
            dskClean.ownWindowsTempFolder();
        }
        /// <summary>
        /// Takes ownership of all the files from the AppData temp folder to the current user.
        /// </summary>
        public void ownAppDataTempFolder()
        {
            dskClean.ownAppDataTempFolder();
        }
        /// <summary>
        /// Takes ownership of all the files from the IE cache folder to the current user.
        /// </summary>
        public void ownIECache()
        {
            dskClean.ownIECacheFolder();
        }
    }
}