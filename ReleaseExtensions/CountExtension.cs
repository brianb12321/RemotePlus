using Logging;
using RemotePlusLibrary.Extension;
using RemotePlusServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReleaseExtensions
{
    public class CountExtension : ServerExtension
    {
        public CountExtension() : base(new ExtensionDetails("CountExtension", "1.0.0.0"))
        {
        }

        public override OperationStatus Execute(ExtensionExecutionContext Context, params object[] arguments)
        {
            OperationStatus s = new OperationStatus();
            List<int> il = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                if (Context.Mode == CallType.GUI)
                {
                    il.Add(i);
                }
                else
                {
                    ServerManager.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, i.ToString(), "CountEstension"));
                }
            }
            s.Data = il;
            return s;
        }

        public override void HaultExtension()
        {
            throw new NotImplementedException();
        }

        public override void ResumeExtension()
        {
            throw new NotImplementedException();
        }
    }
}