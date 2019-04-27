using System;
using System.ServiceModel;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.Security.Authentication;
using BetterLogger;
using RemotePlusLibrary.Core;
using System.Drawing;

namespace RemotePlusLibrary.Contracts
{
    [ServiceKnownType("GetKnownTypes", typeof(DefaultKnownTypeManager))]
    public interface IRemoteClient : IClient, IBidirectionalContract
    {
        [OperationContract(IsOneWay = true)]
        void TellMessage(Guid serverGuid, string Message, LogLevel o);
        [OperationContract(Name = "TellMessageToServerConsoleUsingString")]
        void WriteToClientConsole(Guid serverGuid, string Message);
        [OperationContract(Name = "TellMessageToServerConsoleUsingStringNoNewLine")]
        void WriteToClientConsoleNoNewLine(Guid serverGuid, string Message);
        [OperationContract(Name = "TellMessageToServerConsoleWithLogLevel")]
        void WriteToClientConsole(Guid serverGuid, string Message, LogLevel level);
        [OperationContract(Name = "TellMessageToServerConsoleWithConsoleText")]
        void WriteToClientConsole(Guid serverGuid, ConsoleText text);
        [OperationContract(Name = "TellMessageToServerConsoleWithFrom")]
        void WriteToClientConsole(Guid serverGuid, string Message, LogLevel level, string from);
        [OperationContract]
        void SetClientConsoleBackgroundColor(Guid serverGuid, Color bgColor);
        [OperationContract]
        void SetClientConsoleForegroundColor(Guid serverGuid, Color fgColor);
        [OperationContract]
        void ResetClientConsoleColor(Guid serverGuid);
        [OperationContract]
        void ClearClientConsole(Guid serverGuid);
        [OperationContract]
        ClientBuilder RegisterClient();
        [OperationContract(IsOneWay = true)]
        void Disconnect(Guid serverGuid, string Reason);
        [OperationContract]
        UserCredentials RequestAuthentication(Guid serverGuid, AuthenticationRequest Request);
        [OperationContract]
        ReturnData RequestInformation(Guid serverGuid, RequestBuilder builder);
        [OperationContract(IsOneWay = true)]
        void UpdateRequest(Guid serverGuid, UpdateRequestBuilder message);
        [OperationContract]
        void DisposeCurrentRequest(Guid serverGuid);
        [OperationContract(IsOneWay = true)]
        void RegistirationComplete(Guid serverGuid);
        [OperationContract(IsOneWay = true)]
        void ChangePrompt(Guid serverGuid, PromptBuilder newPrompt);
        [OperationContract]
        PromptBuilder GetCurrentPrompt();
    }
}
