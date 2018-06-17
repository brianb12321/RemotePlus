using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Logging;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Security.AccountSystem;

namespace RemotePlusLibrary
{
    public interface IRemoteClient : IClient, IBidirectionalContract
    {
        [OperationContract(IsOneWay = true)]
        void TellMessage(string Message, Logging.OutputLevel o);
        [OperationContract(Name = "TellMessageToServerConsole")]
        void TellMessageToServerConsole(UILogItem li);
        [OperationContract(Name = "TellMessageToServerConsoleUsingString")]
        void TellMessageToServerConsole(string Message);
        [OperationContract(Name = "TellMessageToServerConsoleConsoleText")]
        void TellMessageToServerConsole(ConsoleText text);
        [OperationContract]
        ClientBuilder RegisterClient();
        [OperationContract(Name = "TellMessageWithLogItem", IsOneWay = true)]
        void TellMessage(UILogItem li);
        [OperationContract(Name = "TellMessageWithLogs", IsOneWay = true)]
        void TellMessage(UILogItem[] Logs);
        [OperationContract(IsOneWay = true)]
        void Disconnect(string Reason);
        [OperationContract]
        UserCredentials RequestAuthentication(AuthenticationRequest Request);
        [OperationContract]
        ReturnData RequestInformation(RequestBuilder builder);
        [OperationContract(IsOneWay = true)]
        void RegistirationComplete();
        [OperationContract(IsOneWay = true)]
        void SendSignal(SignalMessage signal);
        [OperationContract(IsOneWay = true)]
        void ChangePrompt(PromptBuilder newPrompt);
        [OperationContract]
        PromptBuilder GetCurrentPrompt();
    }
}
