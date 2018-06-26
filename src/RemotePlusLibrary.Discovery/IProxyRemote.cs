using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using System;
using System.ServiceModel;

namespace RemotePlusLibrary.Discovery
{
    [ServiceContract(CallbackContract = typeof(IRemoteClient),
        SessionMode = SessionMode.Required)]
    public interface IProxyRemote : IRemote
    {
        [OperationContract(Name = "SelectServerByNumber")]
        void SelectServer(int serverPosition);
        [OperationContract(Name = "SelectServerByGuid")]
        void SelectServer(Guid guid);
        [OperationContract]
        Guid[] GetServers();
        [OperationContract]
        Guid GetSelectedServerGuid();
        [OperationContract]
        void ProxyRegister();
        [OperationContract]
        void ProxyDisconnect();
        [OperationContract]
        CommandPipeline ExecuteProxyCommand(string command, CommandExecutionMode mode);
    }
}