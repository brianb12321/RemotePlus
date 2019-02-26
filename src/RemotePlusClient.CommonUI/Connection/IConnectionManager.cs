using RemotePlusClient.CommonUI.Connection;
using RemotePlusLibrary;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClient.CommonUI.Connection
{
    public interface IConnectionManager
    {
        object ClientCallback { get; set; }
        CommunicationState GetState();
        IRemote GetRemote();
        IProxyRemote GetProxyRemote();
        void Register(RegisterationObject obj);
        void Close();
        void Open(Connection conn);
    }
}
