using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Discovery
{
    [ServiceContract(CallbackContract = typeof(IRemoteClient),
        SessionMode = SessionMode.Required)]
    [ServiceKnownType("GetKnownTypes", typeof(DefaultKnownTypeManager))]
    public interface IProxyRemote : IRemote
    {
        [OperationContract]
        object ExecuteProxyScript(string script);
        [OperationContract(Name = "SelectServerByNumber")]
        void SelectServer(int serverPosition);
        [OperationContract(Name = "SelectServerByGuid")]
        void SelectServer(Guid guid);
        [OperationContract]
        Guid[] GetServers();
        [OperationContract]
        void ProxyRegister();
        [OperationContract]
        void ProxyDisconnect();
        [OperationContract]
        CommandPipeline ExecuteProxyCommand(string command, CommandExecutionMode mode);
        [OperationContract(Name = "ExecuteProxyCommandAsync")]
        Task<CommandPipeline> ExecuteProxyCommandAsync(string command, CommandExecutionMode mode);
    }
}