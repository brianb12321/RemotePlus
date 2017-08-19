using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension;

namespace RemotePlusLibrary.Extension.ExtensionLibraries.InitEnvironments
{
    public class ClientInitEnvironment : IInitEnvironment
    {
        public bool PreviousError { get; private set; }
        public ClientInitEnvironment(bool pe)
        {
            PreviousError = pe;
        }
    }
}
