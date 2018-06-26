using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core
{
    /// <summary>
    /// To be used for building a client or server.
    /// </summary>
    public static class _ConnectionFactory
    {
        public static NetTcpBinding BuildBinding()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            binding.MaxBufferPoolSize = long.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = TimeSpan.MaxValue;
            binding.SendTimeout = TimeSpan.MaxValue;
            binding.MaxConnections = 100;
            return binding;
        }
        public static NetHttpBinding BuildHttpBinding()
        {
            NetHttpBinding binding = new NetHttpBinding();
            binding.MaxBufferPoolSize = long.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = TimeSpan.MaxValue;
            binding.SendTimeout = TimeSpan.MaxValue;
            return binding;
        }
    }
}
