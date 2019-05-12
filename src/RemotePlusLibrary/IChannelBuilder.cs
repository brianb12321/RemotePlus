using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace RemotePlusLibrary
{
    public interface IChannelBuilder<HEAD, TService> where HEAD : IChannelBuilder<HEAD, TService>
        where TService : IRemotePlusCommunicationObject
    {
        
    }
}