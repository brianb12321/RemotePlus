using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Logging;

namespace RemotePlusLibrary
{
    public interface IRemoteClient
    {
        [OperationContract]
        void TellMessage(string Message, Logging.OutputLevel o);
        [OperationContract]
        void TellMessageToServerConsole(LogItem li);
        [OperationContract]
        void AskQuestionToServerConsole(string Question);
        [OperationContract(Name = "TellMessageWithLogItem")]
        void TellMessage(LogItem li);
        [OperationContract(Name = "TellMessageWithLogs")]
        void TellMessage(LogItem[] Logs);
        [OperationContract]
        void Disconnect(string Reason);
        [OperationContract]
        UserCredentials RequestAuthentication();
    }
}
