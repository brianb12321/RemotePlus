using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.ServiceArchitecture;
using System.ServiceModel.Channels;

namespace RemotePlusServer.Core
{
    public class ServerRemotePlusService : StandordService<ServerRemoteInterface>
    {
        public ServerRemotePlusService(object singleTon, Binding binding, string address) : base(typeof(IRemote), singleTon, binding, address)
        {

        }
    }
}