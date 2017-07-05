using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    public delegate int CommandDelegate(string[] args);
    public static class RemotePlusConsole
    {
        public static string ShowHelp(IDictionary<string, CommandDelegate> commands, string[] args)
        {
            string t = "";
            if (args.Length == 2)
            {
                t = ShowHelpPage(commands[args[1]]);
            }
            else if (args.Length >= 1)
            {
                foreach (KeyValuePair<string, CommandDelegate> c in commands)
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
            return t;
        }
        public static string ShowCommandHelpDescription(CommandDelegate command)
        {
            string t = "";
            foreach (object o in command.Method.GetCustomAttributes(false))
            {
                if (o is CommandHelpAttribute)
                {
                    CommandHelpAttribute cha = (CommandHelpAttribute)o;
                    t += cha.HelpMessage;
                }
            }
            return t;
        }
        public static string ShowCommandHelpDescription(IDictionary<string, CommandDelegate> commands, string command)
        {
            string t = "";
            if (commands[command].Method.GetCustomAttributes(false).Length > 0)
            {
                foreach (object o in commands[command].Method.GetCustomAttributes(false))
                {
                    if (o is CommandHelpAttribute)
                    {
                        CommandHelpAttribute cha = (CommandHelpAttribute)o;
                        t += cha.HelpMessage;
                    }
                }
            }
            else
            {
                throw new KeyNotFoundException();
            }
            return t;
        }
        public static string ShowHelpPage(CommandDelegate command)
        {
            string t = "";
            foreach (object a in command.Method.GetCustomAttributes(false))
            {
                if (a is HelpPageAttribute)
                {
                    var a2 = a as HelpPageAttribute;
                    if (a2.Source == HelpSourceType.Text)
                    {
                        t = a2.Details;
                    }
                    else if (a2.Source == HelpSourceType.File)
                    {
                        foreach (string lines in File.ReadAllLines("helpDocs\\" + a2.Details))
                        {
                            t += lines + "\n";
                        }
                    }
                }
            }
            return t;
        }
        public static string ShowHelpPage(IDictionary<string, CommandDelegate> commands, string command)
        {
            string t = "";
            foreach (object a in commands[command].Method.GetCustomAttributes(false))
            {
                if (a is HelpPageAttribute)
                {
                    var a2 = a as HelpPageAttribute;
                    if (a2.Source == HelpSourceType.Text)
                    {
                        t = a2.Details;
                    }
                    else if (a2.Source == HelpSourceType.File)
                    {
                        foreach (string lines in File.ReadAllLines("helpDocs\\" + a2.Details))
                        {
                            t += lines + "\n";
                        }
                    }
                }
            }
            return t;
        }
        public static CommandBehaviorAttribute GetCommandBehavior(CommandDelegate command)
        {
            try
            {
                object a = command.Method.GetCustomAttributes(false).Where(t => t is CommandBehaviorAttribute)
                .First();
                if (a != null)
                {
                    return (CommandBehaviorAttribute)a;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}