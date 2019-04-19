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
    public class HelloWorldActivityModule : IRemotePlusWorkflowModule
    {
        public void InitializeServices(IServiceCollection kernel)
        {
            
        }

        public string WorkflowName => "Hello World Activity";
        public Activity Activity => new TestWorkflowExtensionLibrary.Workflows.HelloWorldWorkflow();
    }
}