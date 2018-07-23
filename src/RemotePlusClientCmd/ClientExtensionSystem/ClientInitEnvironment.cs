using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClientCmd.ClientExtensionSystem
{
    public class ClientInitEnvironment : IInitEnvironment
    {
        public int InitPosition { get; set; }

        public bool PreviousError { get; set; }
        public ClientInitEnvironment(bool pe)
        {
            InitPosition = 1;
            PreviousError = pe;
        }
    }
}
