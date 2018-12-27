using ArduinoRemoteExtensionsLib.Events;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoRemoteExtensions
{
    public class Startup : ILibraryStartup
    {
        public void Init(ILibraryBuilder builder, IInitEnvironment env)
        {
            GlobalServices.Logger.Log("Starting up Arduino Remote Extensions", BetterLogger.LogLevel.Info);
            builder.AddCommandClass<ArduinoRemoteCommands>();
            DefaultKnownTypeManager.AddType(typeof(ArduinoEvent));
        }

        public void PostInit()
        {
            
        }
    }
}