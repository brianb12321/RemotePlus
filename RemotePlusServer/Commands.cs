using Logging;
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
        private static List<LogItem> ProcessStartCommand(string[] args)
        {
            List<LogItem> l = new List<LogItem>();
            if (args.Length > 1)
            {
                string a = "";
                for (int i = 2; i < args.Length; i++)
                {
                    a += " " + args[i];
                }
                Remote.RunProgram((string)args[1], a);
                l.Add(Logger.AddOutput("Program start command finished.", OutputLevel.Info));
            }
            else if (args.Length == 1)
            {
                Remote.RunProgram((string)args[1], "");
                l.Add(Logger.AddOutput("Program start command finished.", OutputLevel.Info));
            }
            return l;
        }
        [CommandHelp("Displays a list of commands.")]
        private static List<LogItem> Help(string[] arguments)
        {
            List<LogItem> l = new List<Logging.LogItem>();
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
            l.Add(new LogItem(OutputLevel.Info, t, "Server Host"));
            return l;
        }
        [CommandHelp("Executes a loaded extension on the server.")]
        private static List<LogItem> ExCommand(object[] args)
        {
            List<LogItem> l = new List<LogItem>();
            List<string> obj = new List<string>();
            for (int i = 2; i < args.Length; i++)
            {
                obj.Add((string)args[i]);
            }
            Remote.RunExtension((string)args[1], obj.ToArray());
            l.Add(Logger.AddOutput("Extension executed.", OutputLevel.Info));
            return l;
        }
        [CommandHelp("Gets the server log.")]
        private static List<LogItem> Logs(object[] args)
        {
            return Logger.buffer;
        }
    }
}
