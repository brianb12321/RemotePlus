using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging;
using RemotePlusServer.Core;
using RemotePlusServer.Core.ServerCore;
using static RemotePlusServer.Core.DefaultCommands;
using RemotePlusServer;

namespace DefaultServerCore
{
    public class Startup : IServerCoreStartup
    {
        void IServerCoreStartup.InitializeServer(IServerBuilder builder)
        {
            builder.InitializeKnownTypes()
                .LoadServerConfig()
                .InitializeDefaultGlobals()
                .InitializeScriptingEngine((options) => { })
                .CreateServer()
                .LoadExtensionLibraries()
                .InitializeVariables()
                .AddTask(() => ServerManager.Logger.AddOutput("Loading Commands.", OutputLevel.Info))
                .AddCommand("ps", ProcessStartCommand)
                .AddCommand("help", Help)
                .AddCommand("logs", Logs)
                .AddCommand("vars", vars)
                .AddCommand("dateTime", dateTime)
                .AddCommand("processes", processes)
                .AddCommand("version", version)
                .AddCommand("encrypt", svm_encyptFile)
                .AddCommand("decrypt", svm_decryptFile)
                .AddCommand("beep", svm_beep)
                .AddCommand("speak", svm_speak)
                .AddCommand("showMessageBox", svm_showMessageBox)
                .AddCommand("path", path)
                .AddCommand("cd", cd)
                .AddCommand("echo", echo)
                .AddCommand("load-extensionLibrary", loadExtensionLibrary)
                .AddCommand("cp", cp)
                .AddCommand("deleteFile", deleteFile)
                .AddCommand("echoFile", echoFile)
                .AddCommand("ls", ls)
                .AddCommand("genMan", genMan)
                .AddCommand("scp", scp)
                .AddCommand("resetStaticScript", resetStaticScript)
                .AddCommand("requestFile", requestFile);
        }
    }
}
