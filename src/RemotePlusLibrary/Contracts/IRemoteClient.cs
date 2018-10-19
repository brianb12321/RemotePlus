using System;
using System.ServiceModel;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Client;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.Security.Authentication;
using BetterLogger;

namespace RemotePlusLibrary.Contracts
{
    public interface IRemoteClient : IClient, IBidirectionalContract
    {
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
        void ChangePrompt(Guid serverGuid, PromptBuilder newPrompt);
        [OperationContract]
        PromptBuilder GetCurrentPrompt();
    }
}
