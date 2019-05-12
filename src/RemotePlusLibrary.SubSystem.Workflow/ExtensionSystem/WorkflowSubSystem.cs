using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;

namespace RemotePlusLibrary.SubSystem.Workflow.ExtensionSystem
{
    public class WorkflowSubSystem : BaseExtensionSubsystem<IWorkflowSubsystem, IRemotePlusWorkflowModule>, IWorkflowSubsystem
    {
        public WorkflowSubSystem(IExtensionLibraryLoader loader) : base(loader)
        {
        }

        public override void Init()
        {
            base.Init();
            GlobalServices.Logger.Log("Workflow Subsystem started.", BetterLogger.LogLevel.Info);
        }

        public CommandResponse RunWorkflow(string name, ICommandEnvironment env, object sender)
        {
            Dictionary<string, object> inputs = new Dictionary<string, object>();
            inputs.Add("EnvGuid", GlobalServices.RunningApplication.EnvironmentGuid);
            inputs.Add("Sender", sender);
            var modules = base.GetAllModules();
            var rightModule = modules.FirstOrDefault(m => m.WorkflowName == name);
            if (rightModule != null)
            {
                WorkflowInvoker invoker = new WorkflowInvoker(rightModule.RunActivity());
                invoker.Extensions.Add(new RemotePlusActivityContext(env, IOCContainer.Provider));
                invoker.Invoke(inputs);
                return new CommandResponse((int)CommandStatus.Success);
            }
            else
            {
                throw new Exception($"Workflow does not exist for name: \"{name}\"");
            }
        }
    }
}