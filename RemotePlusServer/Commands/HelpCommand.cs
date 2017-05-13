using RemotePlusLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging;
using static RemotePlusServer.ServerManager;

namespace RemotePlusServer.Commands
{
    public class HelpCommand : ServerExtension
    {
        public HelpCommand()
        {
            this.Description = "Displays a list of commands.";
        }
        public override List<LogItem> Execute(string[] args)
        {
            List<LogItem> l = new List<Logging.LogItem>();
            string t = "";
            foreach (KeyValuePair<string, ServerExtension> c in ServerManager.Commands)
            {
                if(!string.IsNullOrEmpty(c.Value.Description))
                {
                    t += $"\n{c.Key}\t{c.Value.Description}";
                }
                else
                {
                    t += $"\n{c.Key}";
                }
            }
            l.Add(new LogItem(OutputLevel.Info, t, "Server Host"));
            return l;
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }

        public override void ShowWelcome()
        {
            throw new NotImplementedException();
        }
    }
}
