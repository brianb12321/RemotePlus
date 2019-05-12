using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.ServiceArchitecture;

namespace RemotePlusServer.Core
{
    public class FileTransferServiceBuilder : DefaultServiceBuilder<FileTransferServciceInterface>
    {
        Type _serviceImpl;
        public FileTransferServiceBuilder(Type serviceImpl)
        {
            _serviceImpl = serviceImpl;
        }
        public override IRemotePlusService<FileTransferServciceInterface> BuildService()
        {
            string endpointAddress = "FileTransfer";
            var _service = new FileTransferService(_serviceImpl, _binding, $"net.tcp://0.0.0.0:{_portNumber}/{endpointAddress}");
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

        public override IWCFServiceBuilder<FileTransferServciceInterface> UseSingleton(object singleTon)
        {
            throw new NotImplementedException();
        }
    }
}