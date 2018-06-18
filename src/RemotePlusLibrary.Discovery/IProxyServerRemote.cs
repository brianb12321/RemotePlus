using Logging;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Security.AccountSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Discovery
{
    [ServiceContract(CallbackContract = typeof(IRemote))]
    public interface IProxyServerRemote : IBidirectionalContract
    {
        [OperationContract]
        void Register();
        [OperationContract(IsOneWay = true)]
        void TellMessage(Guid serverGuid, string Message, Logging.OutputLevel o);
        [OperationContract(Name = "TellMessageToServerConsole")]
        void TellMessageToServerConsole(Guid serverGuid, UILogItem li);
        [OperationContract(Name = "TellMessageToServerConsoleUsingString")]
        void TellMessageToServerConsole(Guid serverGuid, string Message);
        [OperationContract(Name = "TellMessageToServerConsoleConsoleText")]
        void TellMessageToServerConsole(Guid serverGuid, ConsoleText text);
        [OperationContract]
        ClientBuilder RegisterClient();
        [OperationContract(Name = "TellMessageWithLogItem", IsOneWay = true)]
        void TellMessage(Guid serverGuid, UILogItem li);
        [OperationContract(Name = "TellMessageWithLogs", IsOneWay = true)]
        void TellMessage(Guid serverGuid, UILogItem[] Logs);
        [OperationContract(IsOneWay = true)]
        void Disconnect(Guid serverGuid, string Reason);
        [OperationContract]
        UserCredentials RequestAuthentication(Guid serverGuid, AuthenticationRequest Request);
        [OperationContract]
        ReturnData RequestInformation(Guid serverGuid, RequestBuilder builder);
        [OperationContract(IsOneWay = true)]
        void RegistirationComplete(Guid serverGuid);
        [OperationContract(IsOneWay = true)]
        void SendSignal(Guid serverGuid, SignalMessage signal);
        [OperationContract(IsOneWay = true)]
        void ChangePrompt(Guid serverGuid, PromptBuilder newPrompt);
        [OperationContract]
        PromptBuilder GetCurrentPrompt();
    }
}
