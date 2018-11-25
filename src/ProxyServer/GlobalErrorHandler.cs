
using BetterLogger;
using Ninject;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.Core.EventSystem.Events;
using RemotePlusLibrary.Core.Faults;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace ProxyServer
{
    public class GlobalErrorHandler : IErrorHandler
    {
        ILogFactory _logger;
        IEventBus _eventBus;
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
            FaultException<ProxyFault> fexp = new FaultException<ProxyFault>(new ProxyFault(ProxyManager.ProxyService.RemoteInterface.SelectedClient.UniqueID), error.Message);
            MessageFault m = fexp.CreateMessageFault();
            fault = Message.CreateMessage(version, m, null);
        }
    }
}