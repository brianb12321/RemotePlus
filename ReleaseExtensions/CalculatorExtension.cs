using RemotePlusLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseExtensions
{
    public class CalculatorExtension : ServerExtension
    {
        public CalculatorExtension() : base(new ExtensionDetails("CalculatorExtension", "1.0.0.0"))
        {
        }

        public override OperationStatus Execute(ExtensionExecutionContext Context, params object[] arguments)
        {
            OperationStatus Status = new OperationStatus();
            switch(arguments[0])
            {
                case "add":
                    if (Context.Mode == CallType.GUI)
                    {
                        Status.Data = double.Parse((string)arguments[1]) + double.Parse((string)arguments[2]);
                        Status.Success = true;
                    }
                    else
                    {
                        RemotePlusServer.ServerManager.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, (double.Parse((string)arguments[1]) + double.Parse((string)arguments[2])).ToString(), "Calculator"));
                    }
                    return Status;
                case "multiply":
                    if (Context.Mode == CallType.GUI)
                    {
                        Status.Data = double.Parse((string)arguments[1]) * double.Parse((string)arguments[2]);
                        Status.Success = true;
                    }
                    else
                    {
                        RemotePlusServer.ServerManager.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, (double.Parse((string)arguments[1]) * double.Parse((string)arguments[2])).ToString(), "Calculator"));
                    }
                    return Status;
                case "divide":
                    if (Context.Mode == CallType.GUI)
                    {
                        Status.Data = double.Parse((string)arguments[1]) / double.Parse((string)arguments[2]);
                        Status.Success = true;
                    }
                    else
                    {
                        RemotePlusServer.ServerManager.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, (double.Parse((string)arguments[1]) / double.Parse((string)arguments[2])).ToString(), "Calculator"));
                    }
                    return Status;
                case "subtract":
                    if (Context.Mode == CallType.GUI)
                    {
                        Status.Data = double.Parse((string)arguments[1]) - double.Parse((string)arguments[2]);
                        Status.Success = true;
                    }
                    else
                    {
                        RemotePlusServer.ServerManager.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, (double.Parse((string)arguments[1]) - double.Parse((string)arguments[2])).ToString(), "Calculator"));
                    }
                    return Status;
                default:
                    if(Context.Mode == CallType.GUI)
                    {
                        Status.Success = false;
                    }
                    return Status;
            }
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
