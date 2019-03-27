using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoRemoteExtensionsProxy
{
    public class Startup : ILibraryStartup
    {
        public void Init(IServiceCollection services)
        {
            DefaultKnownTypeManager.AddType(typeof(ArduinoRemoteExtensionsLib.Events.ArduinoEvent));
        }

        public void PostInit()
        {
            
        }
    }
}
