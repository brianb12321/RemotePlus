using System.Linq;
using NDesk.Options;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.SubSystem.Workflow.ExtensionSystem;
using RemotePlusServer.Core.ExtensionSystem;

namespace RemotePlusLibrary.SubSystem.Workflow.Server
{
    [ExtensionModule]
    public class WorkflowCommands : ServerCommandClass
    {
        private IWorkflowSubsystem _subsystem = null;
        [CommandHelp("Runs the specified workflow.")]
        public CommandResponse runWorkflow(CommandRequest args, CommandPipeline pipe, ICommandEnvironment environment)
        {
            bool showHelp = false;
            OptionSet options = new OptionSet()
                .Add("help|?", "Displays the help screen.", v => showHelp = true);
            var workflowToRun = options.Parse(args.Arguments.Select(a => a.ToString())).ToList();
            workflowToRun.RemoveAt(0);
            if (showHelp)
            {
                options.WriteOptionDescriptions(environment.Out);
                return new CommandResponse((int)CommandStatus.Success);
            }

            return _subsystem.RunWorkflow(workflowToRun[0], environment, this);
        }

        [CommandHelp("Displays all available workflow activities.")]
        public CommandResponse allActivities(CommandRequest args, CommandPipeline pipe, ICommandEnvironment environment)
        {
            foreach (var modules in _subsystem.GetAllModules())
            {
                environment.WriteLine(modules.WorkflowName);
            }
            environment.WriteLine();
            return new CommandResponse((int)CommandStatus.Success);
        }


        public override void InitializeServices(IServiceCollection services)
        {
            _subsystem = services.GetService<IWorkflowSubsystem>();
            Commands.Add("runWorkflow", runWorkflow);
            Commands.Add("allActivities", allActivities);
        }
    }
}