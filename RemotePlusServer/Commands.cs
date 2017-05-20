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

namespace RemotePlusServer
{
    public static partial class ServerManager
    {
        [CommandHelp("Starts a new process on the server.")]
        [HelpPage("ps.txt", Source = HelpSourceType.File)]
        private static int ProcessStartCommand(string[] args)
        {
            if (args.Length > 1)
            {
                string a = "";
                for (int i = 2; i < args.Length; i++)
                {
                    a += " " + args[i];
                }
                Remote.RunProgram(args[1], a);
                Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, "Program start command finished.", "Server Host"));
                return (int)CommandStatus.Success;
            }
            else if (args.Length == 1)
            {
                Remote.RunProgram((string)args[1], "");
                Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, "Program start command finished.", "Server Host"));
                return (int)CommandStatus.Success;
            }
            return (int)CommandStatus.Fail;
        }
        [CommandHelp("Displays a list of commands.")]
        private static int Help(string[] arguments)
        {
            string t = "";
            if (arguments.Length == 2)
            {
                foreach(object a in Commands[arguments[1]].Method.GetCustomAttributes(false))
                {
                    if(a is HelpPageAttribute)
                    {
                        var a2 = a as HelpPageAttribute;
                        if (a2.Source == HelpSourceType.Text)
                        {
                            t = a2.Details;
                        }
                        else if(a2.Source == HelpSourceType.File)
                        {
                            foreach(string lines in File.ReadAllLines("helpDocs\\" + a2.Details))
                            {
                                t += lines + "\n";
                            }
                        }
                    }
                }
            }
            else if(arguments.Length >= 1)
            {
                foreach (KeyValuePair<string, CommandDelgate> c in Commands)
                {
                    if (c.Value.Method.GetCustomAttributes(false).Length > 0)
                    {
                        foreach (object o in c.Value.Method.GetCustomAttributes(false))
                        {
                            if (o is CommandHelpAttribute)
                            {
                                CommandHelpAttribute cha = (CommandHelpAttribute)o;
                                t += $"\n{c.Key}\t{cha.HelpMessage}";
                            }
                        }
                    }
                    else
                    {
                        t += $"\n{c.Key}";
                    }
                }
            }
            Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, t, "Server Host"));
            return (int)CommandStatus.Success;
        }
        [CommandHelp("Executes a loaded extension on the server.")]
        private static int ExCommand(string[] args)
        {
            
            List<string> obj = new List<string>();
            for (int i = 2; i < args.Length; i++)
            {
                obj.Add((string)args[i]);
            }
            Remote.RunExtension((string)args[1], obj.ToArray());
            Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, "Extension executed.", "Server Host"));
            return (int)CommandStatus.Success;
        }
        [CommandHelp("Gets the server log.")]
        private static int Logs(string[] args)
        {
            foreach(var l in Logger.buffer)
            {
                Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(l.Level, l.Message, l.From));
            }
            return (int)CommandStatus.Success;
        }
        [CommandHelp("Manages variables on the server.")]
        private static int vars(string[] args)
        {
            if(args.Length >= 2)
            {
                if(args[1] == "new")
                {
                    if (args.Length >= 4)
                    {
                        string t = "";
                        for(int i = 3; i < args.Length; i++)
                        {
                            t += $" {args[i]}";
                        }
                        Variables.Add(args[2], t);
                        Remote.Client.ClientCallback.TellMessageToServerConsole($"Variable {args[2]} added.");
                        Remote.Client.ClientCallback.TellMessageToServerConsole($"Saving variable file");
                        Variables.Save();
                        Remote.Client.ClientCallback.TellMessageToServerConsole($"Variable file saved.");
                        return (int)CommandStatus.Success;
                    }
                    else
                    {
                        Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "You must provide a value."));
                        return (int)CommandStatus.Fail;
                    }
                }
                else if(args[1] == "view")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine();
                    foreach (KeyValuePair<string, string> v in Variables)
                    {
                        sb.AppendLine($"{v.Key}\t{v.Value}");
                    }
                    Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, sb.ToString()));
                    return (int)CommandStatus.Success;
                }
                else
                {
                    Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "Invalid action."));
                    return (int)CommandStatus.Fail;
                }
            }
            else
            {
                Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "Please provide an action for this command."));
                return (int)CommandStatus.Fail;
            }
        }
        [CommandHelp("Gets the date and time set on the remote server.")]
        private static int dateTime(string[] args)
        {
            Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, DateTime.Now.ToString()));
            return (int)CommandStatus.Success;
        }
        [CommandHelp("Gets the list of processes running on the remote server.")]
        private static int processes(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            foreach(var p in Process.GetProcesses())
            {
                sb.AppendLine($"Name: {p.ProcessName}, ID: {p.Id}, Start Time: {p.StartTime.ToString()}");
            }
            Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, sb.ToString()));
            return (int)CommandStatus.Success;
        }
        [CommandHelp("Manages the watchers on the remote server.")]
        private static int watchers(string[] args)
        {
            if (args.Length >= 2)
            {
                if(args[1] == "run")
                {
                    if (args.Length >= 4)
                    {
                        string t = "";
                        for (int i = 3; i < args.Length; i++)
                        {
                            t += $" {args[i]}";
                        }
                        Remote.StartWatcher(args[2], t);
                        Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, $"Watcher {args[2]} started."));
                        return (int)CommandStatus.Success;
                    }
                    else
                    {
                        Remote.StartWatcher(args[2], null);
                        Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, $"Watcher {args[2]} started."));
                        return (int)CommandStatus.Success;
                    }
                }
                else if(args[1] == "all")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine();
                    foreach (KeyValuePair<string, WatcherBase> w in Watchers)
                    {
                        sb.AppendLine($"{w.Key}");
                    }
                    Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, sb.ToString()));
                    return (int)CommandStatus.Success;
                }
                else if(args[1] == "running")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine();
                    foreach (KeyValuePair<string, WatcherBase> r in Watchers.Where(t => t.Value.Status == WatcherStatus.Running))
                    {
                        sb.AppendLine($"{r.Key}");
                    }
                    return (int)CommandStatus.Success;
                }
                else
                {
                    Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "Invalid action."));
                    return (int)CommandStatus.Fail;
                }
            }
            else
            {
                Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "Please provide an action."));
                return (int)CommandStatus.Fail;
            }
        }
    }
}
