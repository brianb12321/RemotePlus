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
        public bool PreviousError { get; set; }

        public int InitPosition { get; set; }

        public ServerInitEnvironment(bool pe)
        {
            InitPosition = 1;
            PreviousError = pe;
        }
    }
}
