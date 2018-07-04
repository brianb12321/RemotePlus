using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusClientCmd.RequestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Security.Principal;
using RemotePlusServer;
using RemotePlusLibrary;
using System.Windows.Forms;
using System.Drawing;
using RemotePlusLibrary.RequestSystem;
using RemotePlusServer.Core;
using BetterLogger;
using RemotePlusLibrary.IOC;

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
            SMenuBuilder menu = new SMenuBuilder("Please select a file operation below.");
            menu.MenuOptions.Add("Open file");
            menu.MenuOptions.Add("Open directory");
            menu.MenuOptions.Add("View tree");
            menu.MenuOptions.Add("Exit");
            menu.SelectForeground = Color.White;
            menu.SelectBackColor = Color.RoyalBlue;
            while(true)
            {
                var choice = menu.BuildAndSend();
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
            SMenuBuilder menu = new SMenuBuilder($"File Name: {file.Name}\n" + 
                                                 $"Creation Date: {file.CreationTime.ToString()}\n" + 
                                                 $"Last Modified: {file.LastWriteTime.ToString()}\n" +
                                                 $"Is Readonly: {(file.Attributes == FileAttributes.ReadOnly ? "Yes" : "No")}\n" +
                                                 $"Current Owner: {file.GetAccessControl().GetOwner(typeof(NTAccount)).Value}\n" +
                                                 $"Is Hidden: {(file.Attributes == FileAttributes.Hidden ? "Yes" : "No")}\n" +
                                                 $"Is Compressed: {(file.Attributes == FileAttributes.Compressed ? "yes" : "No")}\n\n" +

                                                 "Please select a file operation.");
            menu.MenuOptions.Add("Append");
            menu.MenuOptions.Add("Overwrite");
            menu.MenuOptions.Add("Delete");
            menu.MenuOptions.Add("Copy");
            menu.MenuOptions.Add("Move");
            menu.MenuOptions.Add("Use Windows encryption");
            menu.MenuOptions.Add("Use Gameclub encryption");
            menu.MenuOptions.Add("Set owner");
            menu.MenuOptions.Add("Return to home emnu");
            var choice = menu.BuildAndSend();
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
            string message = (string)ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(new RequestBuilder("rcmd_multitextBox", "Please enter text to append to the file. When finished, hit {ENTER}", null)).Data;
            File.AppendAllText(fullName, message);
            SendMessage($"File {fullName} appended.", LogLevel.Info);
        }
        private static void OverrideFile(string file)
        {
            string message = (string)ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(new RequestBuilder("rcmd_multitextBox", "Please enter text to override. When finished, hit {ENTER}", null)).Data;
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