using System.ServiceModel.Description;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.ServiceArchitecture;

namespace RemotePlusServer.Core
{
    public class MainRemotePlusServiceBuilder : DefaultServiceBuilder<ServerRemoteInterface>
    {
        object _remote;
        public MainRemotePlusServiceBuilder(object remote)
        {
            _remote = remote;
        }
        public override IRemotePlusService<ServerRemoteInterface> BuildService()
        {
            string endpointAddress = "Remote";
            var _service = new ServerRemotePlusService(typeof(IRemote), _remote, _binding, $"net.tcp://0.0.0.0:{_portNumber}/{endpointAddress}");
            ServiceThrottlingBehavior throt = new ServiceThrottlingBehavior();
            throt.MaxConcurrentCalls = int.MaxValue;
            _service.Host.Description.Behaviors.Add(throt);
            _service.HostClosed += _hostClosed;
            _service.HostClosing += _hostClosing;
            _service.HostOpened += _hostOpened;
            _service.HostOpening += _hostOpening;
            _service.HostFaulted += _hostFaulted;
            _service.HostUnknownMessageReceived += _hostUnknown;
            return _service;
        }

        public override IWCFServiceBuilder<ServerRemoteInterface> UseSingleton(object singleTon)
        {
            _remote = singleTon;
            return this;
        }
    }
}