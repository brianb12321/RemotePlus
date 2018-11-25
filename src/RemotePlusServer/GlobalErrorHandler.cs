
using BetterLogger;
using Ninject;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.Core.EventSystem.Events;
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
        private ILogFactory _logger = null;
        private IEventBus _eventBus = null;
        public GlobalErrorHandler(ILogFactory logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }
        public bool HandleError(Exception error)
        {
            _eventBus.Publish(new ServiceFaultErrorEvent(this, error));
#if DEBUG
            _logger.Log("Fault error: " + error.ToString(), LogLevel.Error);
            return true;
#else
            _logger.Log("Fault error: " + error.Message, LogLevel.Error);
            return true;
#endif
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            FaultException<ServerFault> fexp = new FaultException<ServerFault>(new ServerFault(error.StackTrace, ServerManager.DefaultCollection.Libraries.Select(f => f.Value.FriendlyName).ToList()), error.Message);
            MessageFault m = fexp.CreateMessageFault();
            fault = Message.CreateMessage(version, m, null);
        }
    }
}