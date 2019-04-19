using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core.IOC;

namespace RemotePlusLibrary.SubSystem.Workflow.ExtensionSystem
{
    public static class WorkflowServiceCollectionExtensions
    {
        public static IServiceCollection AddWorkflowFeature(this IServiceCollection services)
        {
            return services.AddSingleton<IWorkflowSubsystem, WorkflowSubSystem>();
        }
    }
}