﻿using Logging;
using RemotePlusLibrary.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private static int ExCommand(object[] args)
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
        private static int Logs(object[] args)
        {
            foreach(var l in Logger.buffer)
            {
                Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(l.Level, l.Message, l.From));
            }
            return (int)CommandStatus.Success;
        }
    }
}