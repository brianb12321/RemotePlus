using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;

namespace RemotePlusLibrary.SubSystem.Workflow.ExtensionSystem
{
    public interface IRemotePlusWorkflowModule : IExtensionModule
    {
        string WorkflowName { get; }
        Activity RunActivity();
    }
}