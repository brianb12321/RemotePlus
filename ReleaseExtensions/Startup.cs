using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension;
using RemotePlusServer;

namespace ReleaseExtensions
{
    public class Startup : ILibraryStartup
    {
        void ILibraryStartup.Init()
        {
            ServerManager.Logger.AddOutput("Welcome to \"ReleaseExtension.\" This library contains some useful tools that demonstrates the powers of \"RemotePlus\"", Logging.OutputLevel.Info);
        }
    }
}