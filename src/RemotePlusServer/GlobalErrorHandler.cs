
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.Faults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer
{
    public class GlobalErrorHandler : IErrorHandler
    {
        public bool HandleError(Exception error)
        {
#if DEBUG
            ServerManager.Logger.AddOutput("Fault error: " + error.ToString(), Logging.OutputLevel.Error);
            return true;
#else
            ServerManager.Logger.AddOutput("Fault error: " + error.Message, Logging.OutputLevel.Error);
            return true;
#endif
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            FaultException<ServerFault> fexp = new FaultException<ServerFault>(new ServerFault()
            {
                LoadedServerExtensionLibs = ServerManager.DefaultCollection.Libraries.Select(f => f.Value.FriendlyName).ToList(),
                StackTrace = error.StackTrace
            }, error.Message);
            MessageFault m = fexp.CreateMessageFault();
            fault = Message.CreateMessage(version, m, null);
        }
    }
}
