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
                foreach (object a in commands[args[1]].Method.GetCustomAttributes(false))
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
    }
}
