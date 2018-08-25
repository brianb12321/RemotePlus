using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Diagnostics;
using RemotePlusServer.Core.ExtensionSystem;
using System.IO;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.Scripting.ScriptPackageEngine;
using System.Drawing;
using BetterLogger;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary;
using RemotePlusLibrary.FileTransfer.Service.PackageSystem;
using System.Media;

namespace RemotePlusServer.Core
{
    public static class DefaultCommands
    {
        [CommandHelp("Starts a new process on the server.")]
        [HelpPage("ps.txt", Source = HelpSourceType.File)]
        public static CommandResponse ProcessStartCommand(CommandRequest args, CommandPipeline pipe)
        {
            if (args.Arguments.Count > 2)
            {
                string a = "";
                if (args.Arguments[2].Type == TokenType.QouteBody)
                {
                    a = args.Arguments[2].Value;
                }
                else
                {
                    for (int i = 2; i < args.Arguments.Count; i++)
                    {
                        a += " " + args.Arguments[i];
                    }
                }
                ServerManager.ServerRemoteService.RemoteInterface.RunProgram(args.Arguments[1].Value, a);
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Program start command finished.");
                return new CommandResponse((int)CommandStatus.Success);
            }
            else if (args.Arguments.Count == 2 && args.Arguments[2].Type != TokenType.QouteBody)
            {
                ServerManager.ServerRemoteService.RemoteInterface.RunProgram(args.Arguments[1].Value, "");
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Program start command finished.");
                return new CommandResponse((int)CommandStatus.Success);
            }
            return new CommandResponse((int)CommandStatus.Fail);
        }
        [CommandHelp("Displays a list of commands.")]
        public static CommandResponse Help(CommandRequest args, CommandPipeline pipe)
        {
            string helpString = string.Empty;
            if(args.Arguments.Count == 2)
            {
                helpString = RemotePlusConsole.ShowHelpPage(ServerManager.ServerRemoteService.Commands, args.Arguments[1].Value);
            }
            else
            {
                helpString = RemotePlusConsole.ShowHelp(ServerManager.ServerRemoteService.Commands);
            }
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(helpString);
            var response = new CommandResponse((int)CommandStatus.Success);
            response.Metadata.Add("helpText", helpString);
            return response;
        }
        [CommandHelp("Gets the server log.")]
        [HelpPage("logs.txt", Source = HelpSourceType.File)]
        public static CommandResponse Logs(CommandRequest args, CommandPipeline pipe)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(Console.In.ReadToEnd());
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Manages variables on the server.")]
        [HelpPage("vars.txt", Source = HelpSourceType.File)]
        public static CommandResponse vars(CommandRequest args, CommandPipeline pipe)
        {
            if (args.Arguments.Count >= 2)
            {
                if (args.Arguments[1].Value == "new")
                {
                    if (args.Arguments.Count >= 4)
                    {
                        string t = "";
                        if (args.Arguments[3].Type == TokenType.QouteBody)
                        {
                            t = args.Arguments[3].Value;
                        }
                        else
                        {
                            for (int i = 4; i < args.Arguments.Count; i++)
                            {
                                t += $"{args.Arguments[i]} ";
                            }
                        }
                        ServerManager.ServerRemoteService.Variables.Add(args.Arguments[2].Value, t);
                        ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Variable {args.Arguments[2]} added.");
                        ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Saving variable file");
                        ServerManager.ServerRemoteService.Variables.Save();
                        ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Variable file saved.");
                        return new CommandResponse((int)CommandStatus.Success);
                    }
                    else
                    {
                        ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText("You must provide a value.") { TextColor = Color.Red});
                        return new CommandResponse((int)CommandStatus.Fail);
                    }
                }
                else if (args.Arguments[1].Value == "view")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine();
                    foreach (KeyValuePair<string, string> v in ServerManager.ServerRemoteService.Variables)
                    {
                        sb.AppendLine($"{v.Key}\t{v.Value}");
                    }
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(sb.ToString());
                    return new CommandResponse((int)CommandStatus.Success);
                }
                else
                {
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText("Invalid action." ){TextColor = Color.Red });
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
            else
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Please provide an action for this command.", LogLevel.Error);
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Gets the date and time set on the remote server.")]
        public static CommandResponse dateTime(CommandRequest args, CommandPipeline pipe)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(DateTime.Now.ToString());
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Gets the list of processes running on the remote server.")]
        public static CommandResponse processes(CommandRequest args, CommandPipeline pipe)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            foreach (var p in Process.GetProcesses())
            {
                try
                {
                    sb.AppendLine($"Name: {p.ProcessName}, ID: {p.Id}, Start Time: {p.StartTime.ToString()}");
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"This process can be accessed: {ex.Message}");
                }
            }
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(sb.ToString());
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Returns the server version.")]
        public static CommandResponse version(CommandRequest args, CommandPipeline pipe)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(ServerManager.DefaultSettings.ServerVersion);
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Executes the EncryptFile service method.")]
        public static CommandResponse svm_encyptFile(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                ServerManager.ServerRemoteService.RemoteInterface.EncryptFile(args.Arguments[1].Value, args.Arguments[2].Value);
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (IndexOutOfRangeException)
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText("You need to provide all the information.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Executes the DecryptFile service method.")]
        public static CommandResponse svm_decryptFile(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                ServerManager.ServerRemoteService.RemoteInterface.DecryptFile(args.Arguments[1].Value, args.Arguments[2].Value);
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (IndexOutOfRangeException)
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText("You need to provide all the information.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Wraps around the beep function.")]
        [HelpPage("beep.txt", Source = HelpSourceType.File)]
        public static CommandResponse svm_beep(CommandRequest args, CommandPipeline pipe)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Beep(int.Parse(args.Arguments[1].Value), int.Parse(args.Arguments[2].Value));
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Wraps around the speak function.")]
        public static CommandResponse svm_speak(CommandRequest args, CommandPipeline pipe)
        {
            VoiceAge age = VoiceAge.Adult;
            VoiceGender gender = VoiceGender.Male;
            string message = "";
            if (args.Arguments[1].Value == "vg_male")
            {
                gender = VoiceGender.Male;
            }
            else if (args.Arguments[1].Value == "vg_female")
            {
                gender = VoiceGender.Female;
            }
            else if (args.Arguments[1].Value == "vg_neutral")
            {
                gender = VoiceGender.Neutral;
            }
            else if (args.Arguments[1].Value == "vg_notSet")
            {
                gender = VoiceGender.NotSet;
            }
            else
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText("You must provide a valid voice gender.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            if (args.Arguments[2].Value == "va_adult")
            {
                age = VoiceAge.Adult;
            }
            else if (args.Arguments[2].Value == "va_child")
            {
                age = VoiceAge.Child;
            }
            else if (args.Arguments[2].Value == "va_senior")
            {
                age = VoiceAge.Senior;
            }
            else if (args.Arguments[2].Value == "va_teen")
            {
                age = VoiceAge.Teen;
            }
            else if (args.Arguments[2].Value == "va_notSet")
            {
                age = VoiceAge.NotSet;
            }
            else
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText("You must provide a valid voice age..") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            if (args.Arguments[3].Type == TokenType.QouteBody)
            {
                message = args.Arguments[3].Value;
            }
            else
            {
                for (int i = 3; i < args.Arguments.Count; i++)
                {
                    message += args.Arguments[i] + " ";
                }
            }
            ServerManager.ServerRemoteService.RemoteInterface.Speak(message, gender, age);
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Wraps around the showMessageBox function.")]
        [HelpPage("showMessageBox.txt", Source = HelpSourceType.File)]
        public static CommandResponse svm_showMessageBox(CommandRequest args, CommandPipeline pipe)
        {
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.None;
            string message = "";
            string caption = "";
            if (args.Arguments[1].Value == "b_OK")
            {
                buttons = MessageBoxButtons.OK;
            }
            else if (args.Arguments[1].Value == "b_OK_CANCEL")
            {
                buttons = MessageBoxButtons.OKCancel;
            }
            else if (args.Arguments[1].Value == "b_ABORT_RETRY_IGNORE")
            {
                buttons = MessageBoxButtons.AbortRetryIgnore;
            }
            else if (args.Arguments[1].Value == "b_RETRY_CANCEL")
            {
                buttons = MessageBoxButtons.RetryCancel;
            }
            else if (args.Arguments[1].Value == "b_YES_NO")
            {
                buttons = MessageBoxButtons.YesNo;
            }
            else if (args.Arguments[1].Value == "b_YES_NO_CANCEL")
            {
                buttons = MessageBoxButtons.YesNoCancel;
            }
            else
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText("Please provide a valid MessageBox button.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail) { CustomStatusMessage = "Invalid messageBox button." };
            }
            if (args.Arguments[2].Value == "i_WARNING")
            {
                icon = MessageBoxIcon.Warning;
            }
            else if (args.Arguments[2].Value == "i_STOP")
            {
                icon = MessageBoxIcon.Stop;
            }
            else if (args.Arguments[2].Value == "i_ERROR")
            {
                icon = MessageBoxIcon.Error;
            }
            else if (args.Arguments[2].Value == "i_HAND")
            {
                icon = MessageBoxIcon.Hand;
            }
            else if (args.Arguments[2].Value == "i_INFORMATION")
            {
                icon = MessageBoxIcon.Information;
            }
            else if (args.Arguments[2].Value == "i_QUESTION")
            {
                icon = MessageBoxIcon.Question;
            }
            else if (args.Arguments[2].Value == "i_EXCLAMATION")
            {
                icon = MessageBoxIcon.Exclamation;
            }
            else if (args.Arguments[2].Value == "i_ASTERISK")
            {
                icon = MessageBoxIcon.Asterisk;
            }
            else
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText("Please provide a valid MessageBox icon type.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail) { CustomStatusMessage = "Invalid MessageBox icon" };
            }
            int message_start = 0;
            if (args.Arguments[3].Value == "caption:")
            {
                if (args.Arguments[4].Type == TokenType.QouteBody)
                {
                    caption = args.Arguments[4].Value;
                }
                else
                {
                    for (int i = 4; i < args.Arguments.Count; i++)
                    {
                        if (args.Arguments[i].Value != "message:")
                        {
                            caption += args.Arguments[i] + " ";
                        }
                        else
                        {
                            message_start = i;
                            break;
                        }
                    }
                }
            }
            else
            {
                caption = "RemotePlusServer";
            }
            if (message_start != 0)
            {
                if (args.Arguments[message_start + 1].Type == TokenType.QouteBody)
                {
                    message = args.Arguments[message_start + 1].Value;
                }
                else
                {
                    for (int i = message_start + 1; i < args.Arguments.Count; i++)
                    {
                        message += args.Arguments[i] + " ";
                    }
                }
            }
            else
            {
                message = "";
            }
            var dr = ServerManager.ServerRemoteService.RemoteInterface.ShowMessageBox(message, caption, icon, buttons);
            CommandResponse response = new CommandResponse((int)CommandStatus.Success);
            response.Metadata.Add("Buttons", buttons.ToString());
            response.Metadata.Add("Icon", icon.ToString());
            response.Metadata.Add("Caption", caption);
            response.Metadata.Add("Message", message);
            response.Metadata.Add("Response", dr.ToString());
            return response;
        }
        [CommandHelp("Displays the path of the current server folder.")]
        public static CommandResponse path(CommandRequest args, CommandPipeline pipe)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"The path to the server is {Environment.CurrentDirectory}");
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Changes the current working directory.")]
        public static CommandResponse cd(CommandRequest args, CommandPipeline pipe)
        {
            if (args.Arguments.Count < 2)
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText("You must specify a path to change to.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            else
            {
                ServerManager.ServerRemoteService.RemoteInterface.CurrentPath = args.Arguments[1].Value;
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.ChangePrompt(new RemotePlusLibrary.Extension.CommandSystem.PromptBuilder()
                {
                    Path = ServerManager.ServerRemoteService.RemoteInterface.CurrentPath,
                    AdditionalData = "Current Path"
                });
                return new CommandResponse((int)CommandStatus.Success);
            }
        }
        [CommandHelp("Prints the message to the screen.")]
        public static CommandResponse echo(CommandRequest args, CommandPipeline pipe)
        {
            string message = "";
            if (args.Arguments[1].Type == TokenType.QouteBody)
            {
                message = args.Arguments[1].Value;
            }
            else
            {
                message = args.GetBody();
            }
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(message + Environment.NewLine);
            return new CommandResponse((int)CommandStatus.Success)
            {
                CustomStatusMessage = message
            };
        }
        [CommandHelp("Loads an extension library dll into the system.")]
        public static CommandResponse loadExtensionLibrary(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                string path = string.Empty;
                if (Path.IsPathRooted(args.Arguments[1].Value))
                {
                    path = args.Arguments[1].Value;
                }
                else
                {
                    path = Path.Combine(ServerManager.ServerRemoteService.RemoteInterface.CurrentPath, args.Arguments[1].Value);
                }
                ServerManager.DefaultCollection.LoadExtension(path, (m, o) =>
                {
                    GlobalServices.Logger.Log(m, o);
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(m, o);
                }, new ServerInitEnvironment(false));
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (Exception ex)
            {
                GlobalServices.Logger.Log($"Unable to load extension library: {ex.Message}", LogLevel.Error);
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText($"Unable to load extension library: {ex.Message}") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Copies a specified file to a new file.")]
        public static CommandResponse cp(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                File.Copy(args.Arguments[1].Value, args.Arguments[2].Value);
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch
            {
                return new CommandResponse((int)CommandStatus.Fail);
                throw;
            }
        }
        [CommandHelp("Deletes the specified file. This cannot be reverted.")]
        public static CommandResponse deleteFile(CommandRequest args, CommandPipeline pipe)
        {
            bool autoAccept = args.Arguments.HasFlag("--acceptDisclamer");
            if (File.Exists(args.Arguments[1].Value))
            {
                if (!autoAccept)
                {
                    DialogResult result = (DialogResult)Enum.Parse(typeof(DialogResult), (string)ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(RequestBuilder.RequestMessageBox("You are about to delete a file. THIS OPERATION IS PERMANENT!!", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)).Data);
                    if (result == DialogResult.Yes)
                    {
                        File.Delete(args.Arguments[1].Value);
                        return new CommandResponse((int)CommandStatus.Success);
                    }
                    else
                    {
                        ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Operation canceled.");
                        return new CommandResponse((int)CommandStatus.Fail);
                    }
                }
                else
                {
                    File.Delete(args.Arguments[1].Value);
                    return new CommandResponse((int)CommandStatus.Success);
                }
            }
            else
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText("File does not exist.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Success);
            }
        }
        [CommandHelp("Reads the specified file and prints the contents to the screen.")]
        public static CommandResponse echoFile(CommandRequest args, CommandPipeline pipe)
        {
            if (File.Exists(args.Arguments[1].Value))
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(File.ReadAllText(args.Arguments[1].Value));
                return new CommandResponse((int)CommandStatus.Success);
            }
            else
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText("The file does not exist.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Lists all the files and directories in the current directory.")]
        public static CommandResponse ls(CommandRequest args, CommandPipeline pipe)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string file in Directory.GetFiles(ServerManager.ServerRemoteService.RemoteInterface.CurrentPath))
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText(Path.GetFileName(file) + "\t") { TextColor = Color.LightGray });
            }
            foreach (string directory in Directory.GetDirectories(ServerManager.ServerRemoteService.RemoteInterface.CurrentPath))
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText(directory + "\t") { TextColor = Color.Purple });
            }
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Generates a sample package manifest file")]
        public static CommandResponse genMan(CommandRequest args, CommandPipeline pipe)
        {
            ScriptPackageManifest m = new ScriptPackageManifest();
            m.PackageName = "TestPackage";
            m.ScriptEntryPoint = "main.py";
            m.GenerateManifestToFile(args.Arguments[1].Value);
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Opens a script package and executes the entry-point script.")]
        public static CommandResponse scp(CommandRequest args, CommandPipeline pipe)
        {
            ScriptPackage package = ScriptPackage.Open(args.Arguments[1].Value);
            try
            {
                package.ExecuteScript();
                package.PackageContents.Dispose();
                package = null;
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (Exception ex)
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText($"Unable to execute script package: {ex.Message}") { TextColor = Color.Red });
                package = null;
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Clears all variables and functions from the interactive scripts.")]
        public static CommandResponse resetStaticScript(CommandRequest reqest, CommandPipeline pipe)
        {
            ServerManager.ScriptBuilder.ClearStaticScope();
            return new CommandResponse((int)CommandStatus.Success);
        }
        //Test command only
        [CommandBehavior(IndexCommandInHelp = false)]
        public static CommandResponse requestFile(CommandRequest req, CommandPipeline pipe)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(RequestBuilder.RequestFile());
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Plays an audio file sent by the client.")]
        public static CommandResponse playAudio(CommandRequest req, CommandPipeline pipe)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Going to play audio file.");
            ServerManager.DefaultPackageInventorySelector.GetInventory<FilePackage>("DefaultFileInventory").Receive(package =>
            {
                if(package.FileName.Contains("splash"))
                {
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("I like swimming!");
                }
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Now playing audio file. Name {package.FileName}");
                SoundPlayer sp = new SoundPlayer(package.Data);
                sp.PlaySync();
            });
            RequestBuilder requestPathBuilder = RequestBuilder.RequestFilePath("Select audio file.");
            requestPathBuilder.Metadata["Filter"] = "Wav File (*.wav)|*.wav";
            var path = ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(requestPathBuilder);
            if (path.AcquisitionState ==  RequestState.OK)
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(RequestBuilder.SendFilePackage(path.Data.ToString()));
            }
            return new CommandResponse((int)CommandStatus.Success);
        }
    }
}