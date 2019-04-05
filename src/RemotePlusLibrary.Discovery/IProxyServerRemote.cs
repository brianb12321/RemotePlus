using RemotePlusLibrary.Client;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.SubSystem.Command;
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
using RemotePlusLibrary.Core;
using System.Drawing;

namespace RemotePlusLibrary.Discovery
{
    [ServiceContract(CallbackContract = typeof(IRemoteWithProxy),
        SessionMode = SessionMode.Required)]
    [ServiceKnownType("GetKnownTypes", typeof(DefaultKnownTypeManager))]
    public interface IProxyServerRemote : IBidirectionalContract
    {
        [OperationContract(IsOneWay = true, IsTerminating = true)]
        void Leave(Guid serverGuid);
        [OperationContract(IsInitiating = true)]
        void Register();
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
        void ClearServerConsole(Guid serverGuid);
        [OperationContract]
        ClientBuilder RegisterClient();
        [OperationContract(IsOneWay = true)]
        void Disconnect(Guid serverGuid, string Reason);
        [OperationContract]
        UserCredentials RequestAuthentication(Guid serverGuid, AuthenticationRequest Request);
        [OperationContract]
        ReturnData RequestInformation(Guid serverGuid, RequestBuilder builder);
        [OperationContract]
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
