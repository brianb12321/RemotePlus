using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using Ninject;

namespace RSPM
{
    public static class PackageCommands
    {

        [CommandHelp("Installs a package from the internet.")]
        public static CommandResponse InstallPackage(CommandRequest req, CommandPipeline pipe)
        {
            try
            {
                IPackageManager manager = IOCContainer.Provider.Get<IPackageManager>();
                manager.LoadPackageSources();
                manager.InstallPackage(req.Arguments[1].Value);
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch
            {
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
    }
}