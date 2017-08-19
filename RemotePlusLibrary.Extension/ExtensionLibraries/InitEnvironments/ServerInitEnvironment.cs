using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension;

namespace RemotePlusLibrary.Extension.ExtensionLibraries.InitEnvironments
{
    public class ServerInitEnvironment : IInitEnvironment
    {
        public bool PreviousError { get; private set; }
        public ServerInitEnvironment(bool pe)
        {
            PreviousError = pe;
        }
    }
}
