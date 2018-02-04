using Logging;
using RemotePlusLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.CommandSystem;
using System.Speech.Synthesis;
using System.Windows.Forms;
using RemotePlusLibrary.Core.EmailService;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusServer.ExtensionSystem;

namespace RemotePlusServer
{
    public static partial class ServerManager
    {
        [CommandHelp("Starts a new process on the server.")]
        [HelpPage("ps.txt", Source = HelpSourceType.File)]
        private static CommandResponse ProcessStartCommand(CommandRequest args, CommandPipeline pipe)
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
                DefaultService.Remote.RunProgram(args.Arguments[1].Value, a);
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, "Program start command finished.", "Server Host"));
                return new CommandResponse((int)CommandStatus.Success);
            }
            else if (args.Arguments.Count == 2 && args.Arguments[2].Type != TokenType.QouteBody)
            {
                DefaultService.Remote.RunProgram(args.Arguments[1].Value, "");
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, "Program start command finished.", "Server Host"));
                return new CommandResponse((int)CommandStatus.Success);
            }
            return new CommandResponse((int)CommandStatus.Fail);
        }
        [CommandHelp("Displays a list of commands.")]
        private static CommandResponse Help(CommandRequest args, CommandPipeline pipe)
        {
            var helpString = RemotePlusConsole.ShowHelp(DefaultService.Commands, args.Arguments.Select(f => f.ToString()).ToArray());
            DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, helpString, "Server Host"));
            var response = new CommandResponse((int)CommandStatus.Success);
            response.Metadata.Add("helpText", helpString);
            return response;
        }
        [CommandHelp("Executes a loaded extension on the server.")]
        [HelpPage("ex.txt", Source = HelpSourceType.File)]
        private static CommandResponse ExCommand(CommandRequest args, CommandPipeline pipe)
        {
            var statusCode = DefaultService.Remote.RunExtension((string)args.Arguments[1].Value, new ExtensionExecutionContext(CallType.CommandLine), args.Arguments.Skip(1).Select(f => f.ToString()).ToArray()).ReturnCode;
            if (statusCode != ExtensionStatusCodes.EXTENSION_NOT_FOUND)
            {
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, "Extension executed.", "Server Host"));
            }
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Gets the server log.")]
        [HelpPage("logs.txt", Source = HelpSourceType.File)]
        private static CommandResponse Logs(CommandRequest args, CommandPipeline pipe)
        {
            foreach(var l in Logger.buffer)
            {
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(l.Level, l.Message, l.From));
            }
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Manages variables on the server.")]
        [HelpPage("vars.txt", Source = HelpSourceType.File)]
        private static CommandResponse vars(CommandRequest args, CommandPipeline pipe)
        {
            if(args.Arguments.Count >= 2)
            {
                if(args.Arguments[1].Value == "new")
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
                        DefaultService.Variables.Add(args.Arguments[2].Value, t);
                        DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, $"Variable {args.Arguments[2]} added."));
                        DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, $"Saving variable file"));
                        DefaultService.Variables.Save();
                        DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, $"Variable file saved."));
                        return new CommandResponse((int)CommandStatus.Success);
                    }
                    else
                    {
                        DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "You must provide a value."));
                        return new CommandResponse((int)CommandStatus.Fail);
                    }
                }
                else if(args.Arguments[1].Value == "view")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine();
                    foreach (KeyValuePair<string, string> v in DefaultService.Variables)
                    {
                        sb.AppendLine($"{v.Key}\t{v.Value}");
                    }
                    DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, sb.ToString()));
                    return new CommandResponse((int)CommandStatus.Success);
                }
                else
                {
                    DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "Invalid action."));
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
            else
            {
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "Please provide an action for this command."));
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Gets the date and time set on the remote server.")]
        private static CommandResponse dateTime(CommandRequest args, CommandPipeline pipe)
        {
            DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, DateTime.Now.ToString()));
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Gets the list of processes running on the remote server.")]
        private static CommandResponse processes(CommandRequest args, CommandPipeline pipe)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            foreach(var p in Process.GetProcesses())
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
            DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, sb.ToString()));
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Returns the server version.")]
        private static CommandResponse version(CommandRequest args, CommandPipeline pipe)
        {
            DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, DefaultSettings.ServerVersion));
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Executes the EncryptFile service method.")]
        private static CommandResponse svm_encyptFile(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                DefaultService.Remote.EncryptFile(args.Arguments[1].Value, args.Arguments[2].Value);
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (IndexOutOfRangeException)
            {
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "You need to provide all the information.", "SVM:EncryptFIle"));
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Executes the DecryptFile service method.")]
        private static CommandResponse svm_decryptFile(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                DefaultService.Remote.DecryptFile(args.Arguments[1].Value, args.Arguments[2].Value);
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (IndexOutOfRangeException)
            {
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "You need to provide all the information.", "SVM:EncryptFIle"));
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Wraps around the beep function.")]
        [HelpPage("beep.txt", Source = HelpSourceType.File)]
        private static CommandResponse svm_beep(CommandRequest args, CommandPipeline pipe)
        {
            DefaultService.Remote.Beep(int.Parse(args.Arguments[1].Value), int.Parse(args.Arguments[2].Value));
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Wraps around the speak function.")]
        private static CommandResponse svm_speak(CommandRequest args, CommandPipeline pipe)
        {
            VoiceAge age = VoiceAge.Adult;
            VoiceGender gender = VoiceGender.Male;
            string message = "";
            if(args.Arguments[1].Value == "vg_male")
            {
                gender = VoiceGender.Male;
            }
            else if(args.Arguments[1].Value == "vg_female")
            {
                gender = VoiceGender.Female;
            }
            else if(args.Arguments[1].Value == "vg_neutral")
            {
                gender = VoiceGender.Neutral;
            }
            else if(args.Arguments[1].Value == "vg_notSet")
            {
                gender = VoiceGender.NotSet;
            }
            else
            {
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "You must provide a valid voice gender.", "Server Host"));
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
            else if(args.Arguments[2].Value == "va_teen")
            {
                age = VoiceAge.Teen;
            }
            else if (args.Arguments[2].Value == "va_notSet")
            {
                age = VoiceAge.NotSet;
            }
            else
            {
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "You must provide a valid voice age..", "Server Host"));
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
            DefaultService.Remote.Speak(message, gender, age);
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Wraps around the showMessageBox function.")]
        [HelpPage("showMessageBox.txt", Source = HelpSourceType.File)]
        private static CommandResponse svm_showMessageBox(CommandRequest args, CommandPipeline pipe)
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
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "Please provide a valid MessageBox button.", "Server Host"));
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
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "Please provide a valid MessageBox icon type.", "Server Host"));
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
            if(message_start != 0)
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
            var dr = DefaultService.Remote.ShowMessageBox(message, caption, icon, buttons);
            CommandResponse response = new CommandResponse((int)CommandStatus.Success);
            response.Metadata.Add("Buttons", buttons.ToString());
            response.Metadata.Add("Icon", icon.ToString());
            response.Metadata.Add("Caption", caption);
            response.Metadata.Add("Message", message);
            response.Metadata.Add("Response", dr.ToString());
            return response;
        }
        [CommandHelp("Displays the path of the current server folder.")]
        private static CommandResponse path(CommandRequest args, CommandPipeline pipe)
        {
            DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, $"The path to the server is {Environment.CurrentDirectory}"));
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Sends a sample email.")]
        [CommandBehavior(IndexCommandInHelp = false)]
        private static CommandResponse sample_Email(CommandRequest args, CommandPipeline pipe)
        {
            EmailClient client = new EmailClient(ServerManager.DefaultEmailSettings);
            if(client.SendEmail("This is a test email from RemotePlus", "test email from RemotePlus", out Exception err))
            {
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, "Email sent"));
                return new CommandResponse((int)CommandStatus.Success);
            }
            else
            {
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, $"Unable to send email. {err.Message}"));
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Changes the current working directory.")]
        private static CommandResponse cd(CommandRequest args, CommandPipeline pipe)
        {
            if (args.Arguments.Count < 2)
            {
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "You must specify a path to change to.", "cd"));
                return new CommandResponse((int)CommandStatus.Fail);
            }
            else
            {
                DefaultService.Remote.CurrentPath = args.Arguments[1].Value;
                DefaultService.Remote.Client.ClientCallback.ChangePrompt(new RemotePlusLibrary.Extension.CommandSystem.PromptBuilder()
                {
                    Path = DefaultService.Remote.CurrentPath,
                    AdditionalData = "Current Path"
                });
                return new CommandResponse((int)CommandStatus.Success);
            }
        }
        [CommandHelp("Prints the message to the screen.")]
        private static CommandResponse echo(CommandRequest args, CommandPipeline pipe)
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
            DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(message + Environment.NewLine);
            return new CommandResponse((int)CommandStatus.Success)
            {
                CustomStatusMessage = message
            };
        }
        [CommandHelp("Loads an extension library dll into the system.")]
        private static CommandResponse loadExtensionLibrary(CommandRequest args, CommandPipeline pipe)
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
                    path = Path.Combine(DefaultService.Remote.CurrentPath, args.Arguments[1].Value);
                }
                var lib = ServerExtensionLibrary.LoadServerLibrary(path, (m, o) =>
                {
                    Logger.AddOutput(m, o);
                    DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(o, m));
                }, new ServerInitEnvironment(false));
                DefaultCollection.Libraries.Add(lib.Name, lib);
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (Exception ex)
            {
                Logger.AddOutput($"Unable to load extension library: {ex.Message}", OutputLevel.Exception);
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, $"Unable to load extension library: {ex.Message}"));
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
    }
}