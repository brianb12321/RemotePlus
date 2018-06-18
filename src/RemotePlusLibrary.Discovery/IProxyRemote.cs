using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Discovery
{
    [ServiceContract(CallbackContract = typeof(IRemoteClient))]
    public interface IProxyRemote : IRemote
    {
        [OperationContract(Name = "SelectServerByNumber")]
        void SelectServer(int serverPosition);
        [OperationContract(Name = "SelectServerByGuid")]
        void SelectServer(Guid guid);
        [OperationContract]
        Guid[] GetServers();
        [OperationContract]
        Guid GetSelectedServerGuid();
        [OperationContract]
        void ProxyRegister();
        [OperationContract]
        void ProxyDisconnect();
    }
}