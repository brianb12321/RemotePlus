using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.SubSystem.Workflow.Server.ActivityDesigners;
using RemotePlusServer;
using RemotePlusServer.Core;

namespace RemotePlusLibrary.SubSystem.Workflow.Server.Activities.Requests
{
    [Designer(typeof(ProgressRequestActivityDesigner))]
    public sealed class ProgressRequestActivity : CodeActivity
    {
        public ProgressRequestActivity()
        {
            DisplayName = "Progress Request";
        }
        // Define an activity input argument of type string
        public InArgument<int> Maximum { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            int max = context.GetValue(this.Maximum);
            var client = context.GetExtension<RemotePlusActivityContext>().CurrentCommandEnvironment.ClientContext
                .GetClient<RemoteClient>();
            client.ClientCallback.RequestInformation(new ProgressRequestBuilder()
            {
                Maximum = max
            });
        }
    }
}