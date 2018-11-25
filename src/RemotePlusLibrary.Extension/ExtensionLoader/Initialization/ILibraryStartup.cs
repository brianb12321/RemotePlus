using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ExtensionLoader.Initialization
{
    public interface ILibraryStartup
    {
        void Init(ILibraryBuilder builder, IInitEnvironment env);
    }
}
