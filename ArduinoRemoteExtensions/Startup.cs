using ArduinoRemoteExtensionsLib.Events;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoRemoteExtensions
{
    public class Startup : ILibraryStartup
    {
        public void Init(IServiceCollection services)
        {
            GlobalServices.Logger.Log("Starting up Arduino Remote Extensions", BetterLogger.LogLevel.Info);
            DefaultKnownTypeManager.AddType(typeof(ArduinoEvent));
        }

        public void PostInit()
        {
            
        }
    }
}