using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension;
using Logging;
using RemotePlusLibrary;
using static RemotePlusServer.ServerManager;

namespace RemotePlusServer.Commands
{
    public class PSCommand : ServerExtension
    {
        public PSCommand()
        {
            this.Description = "Starts a new process on the server.";
        }
        public override List<LogItem> Execute(string[] args)
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
                l.Add(ServerManager.Logger.AddOutput("Program start command finished.", OutputLevel.Info));
            }
            else if (args.Length == 1)
            {
                Remote.RunProgram((string)args[1], "");
                l.Add(ServerManager.Logger.AddOutput("Program start command finished.", OutputLevel.Info));
            }
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
