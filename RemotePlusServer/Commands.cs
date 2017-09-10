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
using RemotePlusLibrary.Extension.WatcherSystem;
using System.Speech.Synthesis;
using System.Windows.Forms;
using RemotePlusLibrary.Core.EmailService;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;

namespace RemotePlusServer
{
    public static partial class ServerManager
    {
        [CommandHelp("Starts a new process on the server.")]
        [HelpPage("ps.txt", Source = HelpSourceType.File)]
        private static CommandResponse ProcessStartCommand(CommandRequest args, CommandPipeline pipe)
        {
            if (args.Arguments.Length > 1)
            {
                string a = "";
                for (int i = 2; i < args.Arguments.Length; i++)
                {
                    a += " " + args.Arguments[i];
                }
                DefaultService.Remote.RunProgram(args.Arguments[1], a);
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, "Program start command finished.", "Server Host"));
                return new CommandResponse((int)CommandStatus.Success);
            }
            else if (args.Arguments.Length == 1)
            {
                DefaultService.Remote.RunProgram((string)args.Arguments[1], "");
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, "Program start command finished.", "Server Host"));
                return new CommandResponse((int)CommandStatus.Success);
            }
            return new CommandResponse((int)CommandStatus.Fail);
        }
        [CommandHelp("Displays a list of commands.")]
        private static CommandResponse Help(CommandRequest args, CommandPipeline pipe)
        {
            DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, RemotePlusConsole.ShowHelp(DefaultService.Commands, args.Arguments), "Server Host"));
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Executes a loaded extension on the server.")]
        [HelpPage("ex.txt", Source = HelpSourceType.File)]
        private static CommandResponse ExCommand(CommandRequest args, CommandPipeline pipe)
        {           
            List<string> obj = new List<string>();
            for (int i = 2; i < args.Arguments.Length; i++)
            {
                obj.Add((string)args.Arguments[i]);
            }
            DefaultService.Remote.RunExtension((string)args.Arguments[1], new ExtensionExecutionContext(CallType.CommandLine), obj.ToArray());
            DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, "Extension executed.", "Server Host"));
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
            if(args.Arguments.Length >= 2)
            {
                if(args.Arguments[1] == "new")
                {
                    if (args.Arguments.Length >= 4)
                    {
                        string t = "";
                        for(int i = 3; i < args.Arguments.Length; i++)
                        {
                            t += $" {args.Arguments[i]}";
                        }
                        DefaultService.Variables.Add(args.Arguments[2], t);
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
                else if(args.Arguments[1] == "view")
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
                sb.AppendLine($"Name: {p.ProcessName}, ID: {p.Id}, Start Time: {p.StartTime.ToString()}");
            }
            DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, sb.ToString()));
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Manages the watchers on the remote server.")]
        private static CommandResponse watchers(CommandRequest args, CommandPipeline pipe)
        {
            if (args.Arguments.Length >= 2)
            {
                if(args.Arguments[1] == "run")
                {
                    if (args.Arguments.Length >= 4)
                    {
                        string t = "";
                        for (int i = 3; i < args.Arguments.Length; i++)
                        {
                            t += $"{args.Arguments[i]}";
                        }
                        DefaultService.Remote.StartWatcher(args.Arguments[2], t);
                        DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, $"Watcher {args.Arguments[2]} started."));
                        return new CommandResponse((int)CommandStatus.Success);
                    }
                    else
                    {
                        DefaultService.Remote.StartWatcher(args.Arguments[2], null);
                        DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, $"Watcher {args.Arguments[2]} started."));
                        return new CommandResponse((int)CommandStatus.Success);
                    }
                }
                else if(args.Arguments[1] == "all")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine();
                    foreach (KeyValuePair<string, WatcherBase> w in Watchers)
                    {
                        sb.AppendLine($"{w.Key}");
                    }
                    DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, sb.ToString()));
                    return new CommandResponse((int)CommandStatus.Success);
                }
                else if(args.Arguments[1] == "running")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine();
                    foreach (KeyValuePair<string, WatcherBase> r in Watchers.Where(t => t.Value.Status == WatcherStatus.Running))
                    {
                        sb.AppendLine($"{r.Key}");
                    }
                    DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, "\n" + sb.ToString(), "Server Host"));
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
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "Please provide an action."));
                return new CommandResponse((int)CommandStatus.Fail);
            }
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
                DefaultService.Remote.EncryptFile(args.Arguments[1], args.Arguments[2]);
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
                DefaultService.Remote.DecryptFile(args.Arguments[1], args.Arguments[2]);
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
            DefaultService.Remote.Beep(int.Parse(args.Arguments[1]), int.Parse(args.Arguments[2]));
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Wraps around the speak function.")]
        private static CommandResponse svm_speak(CommandRequest args, CommandPipeline pipe)
        {
            VoiceAge age = VoiceAge.Adult;
            VoiceGender gender = VoiceGender.Male;
            string message = "";
            if(args.Arguments[1] == "vg_male")
            {
                gender = VoiceGender.Male;
            }
            else if(args.Arguments[1] == "vg_female")
            {
                gender = VoiceGender.Female;
            }
            else if(args.Arguments[1] == "vg_neutral")
            {
                gender = VoiceGender.Neutral;
            }
            else if(args.Arguments[1] == "vg_notSet")
            {
                gender = VoiceGender.NotSet;
            }
            else
            {
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "You must provide a valid voice gender.", "Server Host"));
                return new CommandResponse((int)CommandStatus.Fail);
            }
            if (args.Arguments[2] == "va_adult")
            {
                age = VoiceAge.Adult;
            }
            else if (args.Arguments[2] == "va_child")
            {
                age = VoiceAge.Child;
            }
            else if (args.Arguments[2] == "va_senior")
            {
                age = VoiceAge.Senior;
            }
            else if(args.Arguments[2] == "va_teen")
            {
                age = VoiceAge.Teen;
            }
            else if (args.Arguments[2] == "va_notSet")
            {
                age = VoiceAge.NotSet;
            }
            else
            {
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "You must provide a valid voice age..", "Server Host"));
                return new CommandResponse((int)CommandStatus.Fail);
            }
            for (int i = 3; i < args.Arguments.Length; i++)
            {
                message += args.Arguments[i] + " ";
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
            if (args.Arguments[1] == "b_OK")
            {
                buttons = MessageBoxButtons.OK;
            }
            else if (args.Arguments[1] == "b_OK_CANCEL")
            {
                buttons = MessageBoxButtons.OKCancel;
            }
            else if (args.Arguments[1] == "b_ABORT_RETRY_IGNORE")
            {
                buttons = MessageBoxButtons.AbortRetryIgnore;
            }
            else if (args.Arguments[1] == "b_RETRY_CANCEL")
            {
                buttons = MessageBoxButtons.RetryCancel;
            }
            else if (args.Arguments[1] == "b_YES_NO")
            {
                buttons = MessageBoxButtons.YesNo;
            }
            else if (args.Arguments[1] == "b_YES_NO_CANCEL")
            {
                buttons = MessageBoxButtons.YesNoCancel;
            }
            else
            {
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "Please provide a valid MessageBox button.", "Server Host"));
                return new CommandResponse((int)CommandStatus.Fail);
            }
            if (args.Arguments[2] == "i_WARNING")
            {
                icon = MessageBoxIcon.Warning;
            }
            else if (args.Arguments[2] == "i_STOP")
            {
                icon = MessageBoxIcon.Stop;
            }
            else if (args.Arguments[2] == "i_ERROR")
            {
                icon = MessageBoxIcon.Error;
            }
            else if (args.Arguments[2] == "i_HAND")
            {
                icon = MessageBoxIcon.Hand;
            }
            else if (args.Arguments[2] == "i_INFORMATION")
            {
                icon = MessageBoxIcon.Information;
            }
            else if (args.Arguments[2] == "i_QUESTION")
            {
                icon = MessageBoxIcon.Question;
            }
            else if (args.Arguments[2] == "i_EXCLAMATION")
            {
                icon = MessageBoxIcon.Exclamation;
            }
            else if (args.Arguments[2] == "i_ASTERISK")
            {
                icon = MessageBoxIcon.Asterisk;
            }
            else
            {
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "Please provide a valid MessageBox icon type.", "Server Host"));
                return new CommandResponse((int)CommandStatus.Fail);
            }
            int message_start = 0;
            if (args.Arguments[3] == "caption:")
            {
                for (int i = 4; i < args.Arguments.Length; i++)
                {
                    if (args.Arguments[i] != "message:")
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
            else
            {
                caption = "RemotePlusServer";
            }
            if(message_start != 0)
            {
                for(int i = message_start + 1; i < args.Arguments.Length; i++)
                {
                    message += args.Arguments[i] + " ";
                }
            }
            else
            {
                message = "";
            }
            DefaultService.Remote.ShowMessageBox(message, caption, icon, buttons);
            return new CommandResponse((int)CommandStatus.Success);
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
    }
}