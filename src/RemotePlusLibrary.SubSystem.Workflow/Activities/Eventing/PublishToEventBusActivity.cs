using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.SubSystem.Workflow.ActivityDesigners;

namespace RemotePlusLibrary.SubSystem.Workflow.Activities.Eventing
{
    [Designer(typeof(PublishToEventBusActivityDesigner))]
    public sealed class PublishToEventBusActivity : CodeActivity
    {
        // Define an activity input argument of type string
        [RequiredArgument]
        public InArgument<TinyMessenger.ITinyMessage> EventMessage { get; set; }
        public InArgument<bool> PrivatePublish { get; set; }

        public PublishToEventBusActivity()
        {
            DisplayName = "Publish to EventBus";
            PrivatePublish = false;
        }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            var eventMessage = context.GetValue(this.EventMessage);
            var privatePublish = context.GetValue(this.PrivatePublish);
            IEventBus bus = context.GetExtension<RemotePlusActivityContext>().ServiceCollection.GetService<IEventBus>();
            if(privatePublish) bus.PublishPrivate(eventMessage);
            else bus.Publish(eventMessage);
        }
    }
}