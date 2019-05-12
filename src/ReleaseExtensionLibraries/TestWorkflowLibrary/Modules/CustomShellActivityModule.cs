using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.SubSystem.Workflow.ExtensionSystem;

namespace TestWorkflowExtensionLibrary.Modules
{
    [ExtensionModule]
    public class CustomShellActivityModule : IRemotePlusWorkflowModule
    {
        public void InitializeServices(IServiceCollection kernel)
        {
            
        }

        public Activity RunActivity()
        {
            return new Workflows.CustomShellActivity();
        }
        public string WorkflowName => "CustomShellActivity";
    }
}
