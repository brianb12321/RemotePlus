using RemotePlusLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging;

namespace RemotePlusServer.Commands
{
    public class ExCommand : ServerExtension
    {
        public ExCommand() : base(new ExtensionDetails("ex", "1.0.0.0", "Executes a loaded extension on the server."))
        {
        }

        public override OperationStatus Execute(params object[] arguments)
        {
            OperationStatus l = new OperationStatus();
            List<string> obj = new List<string>();
            for (int i = 2; i < arguments.Length; i++)
            {
                obj.Add((string)arguments[i]);
            }
            ServerManager.Remote.RunExtension((string)arguments[1], obj.ToArray());
            l.Log.Add(ServerManager.Logger.AddOutput("Extension executed.", OutputLevel.Info));
            return l;
        }

        public override void HaultExtension()
        {
            throw new NotImplementedException();
        }

        public override void ResumeExtension()
        {
            throw new NotImplementedException();
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
