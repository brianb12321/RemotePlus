using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.SubSystem.Workflow.Server.Activities;
using RemotePlusLibrary.SubSystem.Workflow.Server.ActivityDesigners;
using RemotePlusServer;
using RemotePlusServer.Core;

namespace RemotePlusLibrary.SubSystem.Workflow.Server.Activities.Requests
{
    [Designer(typeof(PromptRequestActivityDesigner))]
    public sealed class PromptRequestActivity : CodeActivity<string>
    {
        public PromptRequestActivity()
        {
            DisplayName = "Prompt Request";
        }
        // Define an activity input argument of type string
        public InArgument<string> Prompt { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override string Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            var remotePlusContext = context.GetExtension<RemotePlusActivityContext>();
            string prompt = context.GetValue(this.Prompt);
            var client = remotePlusContext.CurrentCommandEnvironment.ClientContext.GetClient<RemoteClient>();
            var result = client.ClientCallback.RequestInformation(
                new ConsoleReadLineRequestBuilder(prompt)).Data.ToString();
            return result;
        }
    }
}