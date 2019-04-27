using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Activities.Presentation.Toolbox;
using System.ComponentModel;
using RemotePlusLibrary.RequestSystem.DefaultUpdateRequestBuilders;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.SubSystem.Workflow.Server.ActivityDesigners;
using RemotePlusServer;
using RemotePlusServer.Core;

namespace RemotePlusLibrary.SubSystem.Workflow.Server.Activities.Requests
{
    public sealed class DisposeRequestActivity : CodeActivity
    {
        public DisposeRequestActivity()
        {
            DisplayName = "Dispose Request";
        }
        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            var env = context.GetExtension<RemotePlusActivityContext>().CurrentCommandEnvironment;
            env.ClientContext.GetClient<RemoteClient>().ClientCallback.DisposeCurrentRequest();
        }
    }
}