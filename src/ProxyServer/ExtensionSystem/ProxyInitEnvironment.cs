using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;

namespace ProxyServer.ExtensionSystem
{
    public class ProxyInitEnvironment : IInitEnvironment
    {
        public int InitPosition { get; set; }

        public bool PreviousError { get; set; }
        public ProxyInitEnvironment(bool previousError)
        {
            PreviousError = previousError;
        }
    }
}
