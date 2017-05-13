using RemotePlusLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging;

namespace RemotePlusServer.Commands
{
    public class LogsCommand : ServerExtension
    {
        public LogsCommand()
        {
            this.Description = "Gets the server log.";
        }
        public override List<LogItem> Execute(string[] args)
        {
            return ServerManager.Logger.buffer;
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
