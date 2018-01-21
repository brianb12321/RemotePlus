using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension;
using RemotePlusServer;

namespace ServerMath
{
    public class Startup : ILibraryStartup
    {
        public void Init(ILibraryBuilder builder, IInitEnvironment env)
        {
            ServerManager.Logger.AddOutput("Registering math commands.", Logging.OutputLevel.Info, builder.FriendlyName);
            ServerManager.DefaultService.Commands.Add("abs", MathCommands.abs);
            ServerManager.DefaultService.Commands.Add("pow", MathCommands.pow);
        }
    }
}
