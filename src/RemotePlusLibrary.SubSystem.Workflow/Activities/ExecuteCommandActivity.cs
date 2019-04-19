using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.SubSystem.Workflow.ActivityDesigners;

namespace RemotePlusLibrary.SubSystem.Workflow.Activities
{
    [Designer(typeof(ExecuteCommandActivityDesigner))]
    public sealed class ExecuteCommandActivity : CodeActivity<CommandPipeline>
    {
        // Define an activity input argument of type string
        [RequiredArgument]
        public InArgument<string> Command { get; set; }
        public InArgument<CommandExecutionMode> Mode { get; set; }
        public InArgument<bool> UseCurrentEnvironment { get; set; }

        public ExecuteCommandActivity()
        {
            DisplayName = "Execute Command";
            Mode = CommandExecutionMode.Script;
            UseCurrentEnvironment = true;
        }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override CommandPipeline Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            string command = context.GetValue(this.Command);
            CommandExecutionMode mode = context.GetValue(this.Mode);
            bool useCurrentEnvironment = context.GetValue(this.UseCurrentEnvironment);
            ICommandEnvironment env = null;
            RemotePlusActivityContext rpContext = context.GetExtension<RemotePlusActivityContext>();
            if (!useCurrentEnvironment)
            {
                env = rpContext.ServiceCollection.GetService<ICommandEnvironment>();
            }
            else env = rpContext.CurrentCommandEnvironment;
            var result = env.Execute(command, mode);
            return result;
        }
    }
}