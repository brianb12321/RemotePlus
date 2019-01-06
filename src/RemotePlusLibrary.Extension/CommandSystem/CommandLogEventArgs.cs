namespace RemotePlusLibrary.Extension.CommandSystem
{
    public class CommandLogEventArgs
    {
        public ConsoleText Text { get; private set; }
        public CommandLogEventArgs(ConsoleText text)
        {
            Text = text;
        }
    }
}