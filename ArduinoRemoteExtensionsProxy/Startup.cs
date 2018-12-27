using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoRemoteExtensionsProxy
{
    public class Startup : ILibraryStartup
    {
        public void Init(ILibraryBuilder builder, IInitEnvironment env)
        {
            DefaultKnownTypeManager.AddType(typeof(ArduinoRemoteExtensionsLib.Events.ArduinoEvent));
        }

        public void PostInit()
        {
            
        }
    }
}
