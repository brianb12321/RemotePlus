
using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Core.Faults;
using RemotePlusServer.Core;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace RemotePlusServer
{
    public class GlobalErrorHandler : IErrorHandler
    {
        public bool HandleError(Exception error)
        {
#if DEBUG
            GlobalServices.Logger.Log("Fault error: " + error.ToString(), LogLevel.Error);
            return true;
#else
            GlobalServices.Logger.Log("Fault error: " + error.Message, LogLevel.Error);
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
