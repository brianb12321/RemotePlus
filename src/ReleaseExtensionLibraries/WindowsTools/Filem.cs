using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using System;
using System.IO;
using System.Threading;
using System.Security.Principal;
using RemotePlusLibrary.Core;
using System.Windows.Forms;
using System.Drawing;
using RemotePlusLibrary.RequestSystem;
using RemotePlusServer.Core;
using BetterLogger;
using RemotePlusLibrary.RequestSystem.DefaultRequestOptions;

namespace WindowsTools
{
    /// <summary>
    /// A tool that allows you to manage files on the remote server. It is only a command line tool.
    /// </summary>
    public static class Filem
    {
        const int DELAY_TIMER = 3000;
        static void SendMessage(string message, LogLevel level)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(message, level, "FileM");
            GlobalServices.Logger.Log(message, level, "FileM");
        }
        [CommandHelp("Allows you to manage files on the remote file system.")]
        [CommandBehavior(SupportClients = RemotePlusLibrary.Extension.ClientSupportedTypes.CommandLine,
            ClientRejectionMessage = "FileM is not supported for GUI clients yet.",
            CommandDevelepmentState = RemotePlusLibrary.Extension.ExtensionDevelopmentState.InDevelopment)]
        public static CommandResponse filem_command(CommandRequest args, CommandPipeline pipe)
        {
            //if(ServerManager.ServerRemoteService.RemoteInterface.Client.ClientType != ClientType.CommandLine)
            //{
            //    SendMessage("FileM is currently only availible to command line users.", LogLevel.Error);
            //    return new CommandResponse(-999); // Random Error Code
            //}
            SMenuRequestOptions menu = new SMenuRequestOptions();
            menu.Message = "Please select a file operation below.";
            menu.MenuItems.Add("0", "Open file");
            menu.MenuItems.Add("1", "Open directory");
            menu.MenuItems.Add("2", "View tree");
            menu.MenuItems.Add("3", "Exit");
            menu.SelectForeground = Color.White.ToArgb();
            menu.SelectBackColor = Color.RoyalBlue.ToArgb();
            RequestBuilder rb = new RequestBuilder("rcmd_csmenu");
            rb.PutObject(menu);
            while(true)
            {
                var choice = int.Parse(ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(rb).Data.ToString());
                switch (choice)
                {
                    case 0:
                        openFile();
                        break;
                    case 3:
                        return new CommandResponse((int)CommandStatus.Success);
                }
                Thread.Sleep(DELAY_TIMER);
            }
        }
        static void openFile()
        {
            string file = (string)ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(RequestBuilder.RequestFile()).Data;
            //var filePath = new CmdTextBox("Enter file path to open").BuildAndSend();
            try
            {
                showFIleMenu(new FileInfo(file));

            }
            catch(FileNotFoundException)
            {
                SendMessage($"The file {file} cannot be opened. It does not exist.", LogLevel.Error);
            }
        }
        static void showFIleMenu(FileInfo file)
        {
            SMenuRequestOptions menu = new SMenuRequestOptions();
                                  menu.Message = $"File Name: {file.Name}\n" + 
                                                 $"Creation Date: {file.CreationTime.ToString()}\n" + 
                                                 $"Last Modified: {file.LastWriteTime.ToString()}\n" +
                                                 $"Is Readonly: {(file.Attributes == FileAttributes.ReadOnly ? "Yes" : "No")}\n" +
                                                 $"Current Owner: {file.GetAccessControl().GetOwner(typeof(NTAccount)).Value}\n" +
                                                 $"Is Hidden: {(file.Attributes == FileAttributes.Hidden ? "Yes" : "No")}\n" +
                                                 $"Is Compressed: {(file.Attributes == FileAttributes.Compressed ? "yes" : "No")}\n\n" +

                                                 "Please select a file operation.";
            menu.MenuItems.Add("0", "Append");
            menu.MenuItems.Add("1", "Overwrite");
            menu.MenuItems.Add("2", "Delete");
            menu.MenuItems.Add("3", "Copy");
            menu.MenuItems.Add("4", "Move");
            menu.MenuItems.Add("5", "Use Windows encryption");
            menu.MenuItems.Add("6", "Use Gameclub encryption");
            menu.MenuItems.Add("7", "Set owner");
            menu.MenuItems.Add("8", "Return to home emnu");
            RequestBuilder rb = new RequestBuilder("rcmd_csmenu");
            rb.PutObject(menu);
            var choice = int.Parse(ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(rb).Data.ToString());
            switch(choice)
            {
                case 0:
                    AppendFile(file.FullName);
                    break;
                case 1:
                    OverrideFile(file.FullName);
                    break;
                case 2:
                    DeleteFile(file.FullName);
                    break;
                case 8:
                    return;
            }
        }

        private static void AppendFile(string fullName)
        {
            var rb = new RequestBuilder("rcmd_mTextBox");
            rb.PutObject(new PromptRequestOptions()
            {
                Message = "Please enter text to append to the file. When finished, hit {ENTER}"
            });
            string message = (string)ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(rb).Data;
            File.AppendAllText(fullName, message);
            SendMessage($"File {fullName} appended.", LogLevel.Info);
        }
        private static void OverrideFile(string file)
        {
            var rb = new RequestBuilder("rcmd_mTextBox");
            rb.PutObject(new PromptRequestOptions()
            {
                Message = "Please enter text to override. When finished, hit {ENTER}"
            });
            string message = (string)ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(rb).Data;
            File.WriteAllText(file, message);
            SendMessage($"File {file} overwritten.", LogLevel.Info);
        }

        static void DeleteFile(string file)
        {
            var result = (DialogResult)Enum.Parse(typeof(DialogResult), (string)ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(RequestBuilder.RequestMessageBox($"Are you sure that you want to delete {Path.GetFileName(file)}? You cannot revert once completed.", "File Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)).Data);
            if(result == DialogResult.Yes)
            {
                SendMessage($"Deleting {file}.", LogLevel.Info);
                File.Delete(file);
                SendMessage("File deleted.", LogLevel.Info);
            }
            else
            {
                SendMessage($"The deletion of {Path.GetFileName(file)} was canceled.", LogLevel.Info);
            }
        }
    }
}