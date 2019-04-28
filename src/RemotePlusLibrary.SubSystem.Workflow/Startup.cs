using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.SubSystem.Workflow.ExtensionSystem;

namespace RemotePlusLibrary.SubSystem.Workflow
{
    public class Startup : ILibraryStartup
    {
        public void Init(IServiceCollection services)
        {
            services.GetService<IWorkflowSubsystem>().Init();
        }

        public void PostInit()
        {
            
        }
    }
}