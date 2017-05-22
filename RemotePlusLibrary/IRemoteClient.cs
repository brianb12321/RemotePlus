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
        [OperationContract(IsOneWay = true)]
        void TellMessage(string Message, Logging.OutputLevel o);
        [OperationContract(Name = "TellMessageToServerConsole", IsOneWay = true)]
        void TellMessageToServerConsole(UILogItem li);
        [OperationContract(Name = "TellMessageToServerConsoleUsingString", IsOneWay = true)]
        void TellMessageToServerConsole(string Message);
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
        [OperationContract(IsOneWay = true)]
        void UpdateClientExtension(string ExtensionName, object Data);
    }
}
