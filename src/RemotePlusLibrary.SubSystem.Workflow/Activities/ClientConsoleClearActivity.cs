using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace RemotePlusLibrary.SubSystem.Workflow.Activities
{

    public sealed class ClientConsoleClearActivity : CodeActivity
    {

        public ClientConsoleClearActivity()
        {
            DisplayName = "Client Console Clear";
        }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            context.GetExtension<RemotePlusActivityContext>().CurrentCommandEnvironment.Clear();   
        }
    }
}
