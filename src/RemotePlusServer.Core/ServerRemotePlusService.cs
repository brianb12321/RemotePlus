using System;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.ServiceArchitecture;
using System.ServiceModel.Channels;

namespace RemotePlusServer.Core
{
    public class ServerRemotePlusService : StandordService<ServerRemoteInterface>
    {
        public ServerRemotePlusService(Type impl, Binding binding, string address) : base(typeof(IRemote), impl, binding, address)
        {
            
        }
    }
}