using RemotePlusLibrary.Client;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Security.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using BetterLogger;

namespace RemotePlusLibrary.Discovery
{
    [ServiceContract(CallbackContract = typeof(IRemoteWithProxy),
        SessionMode = SessionMode.Required)]
    public interface IProxyServerRemote : IBidirectionalContract
    {
        [OperationContract(IsOneWay = true, IsTerminating = true)]
        void Leave(Guid serverGuid);
        [OperationContract(IsInitiating = true)]
        void Register();
        [OperationContract(IsOneWay = true)]
        void TellMessage(Guid serverGuid, string Message, LogLevel o);
        [OperationContract(Name = "TellMessageToServerConsoleUsingString")]
        void TellMessageToServerConsole(Guid serverGuid, string Message);
        [OperationContract(Name = "TellMessageToServerConsoleUsingStringNoNewLine")]
        void TellMessageToServerConsoleNoNewLine(Guid serverGuid, string Message);
        [OperationContract(Name = "TellMessageToServerConsoleWithLogLevel")]
        void TellMessageToServerConsole(Guid serverGuid, string Message, LogLevel level);
        [OperationContract(Name = "TellMessageToServerConsoleWithConsoleText")]
        void TellMessageToServerConsole(Guid serverGuid, ConsoleText text);
        [OperationContract(Name = "TellMessageToServerConsoleWithFrom")]
        void TellMessageToServerConsole(Guid serverGuid, string Message, LogLevel level, string from);
        [OperationContract]
        ClientBuilder RegisterClient();
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
