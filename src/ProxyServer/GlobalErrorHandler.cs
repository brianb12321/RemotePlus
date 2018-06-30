
using BetterLogger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.Faults;
using RemotePlusLibrary.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServer
{
    public class GlobalErrorHandler : IErrorHandler
    {
        public bool HandleError(Exception error)
        {
#if DEBUG
            ProxyManager.Logger.Log("Fault error: " + error.ToString(), LogLevel.Error);
            return true;
#else
            ProxyManager.Logger.Log("Fault error: " + error.Message, LogLevel.Error);
            return true;
#endif
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            FaultException<ProxyFault> fexp = new FaultException<ProxyFault>(new ProxyFault(ProxyManager.ProxyService.RemoteInterface.SelectedClient.UniqueID), error.Message);
            MessageFault m = fexp.CreateMessageFault();
            fault = Message.CreateMessage(version, m, null);
        }
    }
}