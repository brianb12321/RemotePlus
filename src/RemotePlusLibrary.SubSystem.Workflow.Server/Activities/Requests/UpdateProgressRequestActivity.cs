using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using RemotePlusLibrary.RequestSystem.DefaultUpdateRequestBuilders;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.SubSystem.Workflow.Server.ActivityDesigners;
using RemotePlusServer;
using RemotePlusServer.Core;

namespace RemotePlusLibrary.SubSystem.Workflow.Server.Activities.Requests
{
    [Designer(typeof(UpdateProgressRequestActivityDesigner))]
    public sealed class UpdateProgressRequestActivity : CodeActivity
    {
        public UpdateProgressRequestActivity()
        {
            DisplayName = "Update Progress Request";
        }
        // Define an activity input argument of type string
        public InArgument<string> Text { get; set; }
        [RequiredArgument]
        public InArgument<int> Value { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            string text = context.GetValue(this.Text);
            int value = context.GetValue(this.Value);
            var client = context.GetExtension<RemotePlusActivityContext>().CurrentCommandEnvironment.ClientContext.GetClient<RemoteClient>();
            client.ClientCallback.UpdateRequest(new ProgressUpdateBuilder(value)
            {
                Text = text
            });
        }
    }
}