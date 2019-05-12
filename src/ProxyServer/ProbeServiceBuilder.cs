using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.ServiceArchitecture;

namespace ProxyServer
{
    public class ProbeServiceBuilder : DefaultServiceBuilder<ProxyServerRemoteImpl>
    {
        ProxyServerRemoteImpl _remote;
        Binding _clientBinding;
        public ProbeServiceBuilder(ProxyServerRemoteImpl remote, Binding clientBinding)
        {
            _remote = remote;
            _clientBinding = clientBinding;
        }
        public override IRemotePlusService<ProxyServerRemoteImpl> BuildService()
        {
            string endpointAddress = "Proxy";
            var _service = new ProbeService(_remote, _binding, _clientBinding, $"net.tcp://0.0.0.0:{_portNumber}/{endpointAddress}", $"net.tcp://0.0.0.0:{_portNumber}/ProxyClient");
            ServiceThrottlingBehavior throt = new ServiceThrottlingBehavior();
            throt.MaxConcurrentCalls = int.MaxValue;
            _service.Behaviors.Add(throt);
            _service.HostClosed += _hostClosed;
            _service.HostClosing += _hostClosing;
            _service.HostOpened += _hostOpened;
            _service.HostOpening += _hostOpening;
            _service.HostFaulted += _hostFaulted;
            _service.HostUnknownMessageReceived += _hostUnknown;
            _service.Behaviors = _behaviors;
            _service.EndpointBehaviors = _endpointBehaviors;
            _service.ContractBehaviors = _contractBehaviors;
            return _service;
        }
        public override IWCFServiceBuilder<ProxyServerRemoteImpl> UseSingleton(object singleTon)
        {
            if(singleTon is ProxyServerRemoteImpl)
            {
                _remote = singleTon as ProxyServerRemoteImpl;
            }
            else
            {
                throw new ArgumentException("The singleton must be of type " + nameof(ProxyServerRemoteImpl));
            }
            return this;
        }
    }
}