
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
            ServerManager.Logger.AddOutput("Fault error: " + error.Message, Logging.OutputLevel.Info);
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            FaultException fexp = new FaultException(error.Message);
            MessageFault m = fexp.CreateMessageFault();
            fault = Message.CreateMessage(version, m, null);
        }
    }
}
