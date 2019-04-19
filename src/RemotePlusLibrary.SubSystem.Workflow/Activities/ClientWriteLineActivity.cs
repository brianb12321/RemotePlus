using System.Activities;
using System.ComponentModel;
using System.Drawing;
using RemotePlusLibrary.SubSystem.Command;

namespace RemotePlusLibrary.SubSystem.Workflow.Activities
{
    [Designer(typeof(Workflow.ActivityDesigners.ClientWriteLineActivityDesigner))]
    public sealed class ClientWriteLineActivity : CodeActivity
    {
        public ClientWriteLineActivity()
        {
            DisplayName = "Client WriteLine";
        }
        // Define an activity input argument of type string
        [RequiredArgument]
        public InArgument<string> Text { get; set; }
        public InArgument<Color> BackgroundColor { get; set; } = Color.Empty;
        public InArgument<Color> ForegroundColor { get; set; } = Color.Empty;

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            string text = context.GetValue(this.Text);
            Color bgColor = context.GetValue(this.BackgroundColor);
            Color fgColor = context.GetValue(this.ForegroundColor);
            RemotePlusActivityContext remotePlusContext = context.GetExtension<RemotePlusActivityContext>();
            remotePlusContext.CurrentCommandEnvironment.WriteLineWithColor(text, fgColor, bgColor);
        }
    }
}
