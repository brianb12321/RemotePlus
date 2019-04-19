using RemotePlusLibrary.Extension;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;

namespace RemotePlusLibrary.SubSystem.Workflow.ExtensionSystem
{
    public interface IWorkflowSubsystem : IExtensionSubsystem<IRemotePlusWorkflowModule>
    {
        CommandResponse RunWorkflow(string name, ICommandEnvironment env);
    }
}