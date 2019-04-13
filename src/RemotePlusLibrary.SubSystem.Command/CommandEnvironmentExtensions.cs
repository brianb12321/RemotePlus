using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command
{
    public static class CommandEnvironmentExtensions
    {
        public static void WriteLineWithColor(this ICommandEnvironment env, string message, Color foregroundColor)
        {
            env.WriteLine(new ConsoleText(message) { TextColor = foregroundColor });
        }
        public static void WriteLineWithColor(this ICommandEnvironment env, string message, Color foregroundColor, Color backgroundColor)
        {
            if (foregroundColor == Color.Empty)
            {
                env.WriteLine(new ConsoleText(message) {BackColor = backgroundColor });
            }
            else env.WriteLine(new ConsoleText(message) { TextColor = foregroundColor, BackColor = backgroundColor });
        }
        public static void WriteLineErrorWithColor(this ICommandEnvironment env, string message, Color foregroundColor)
        {
            env.WriteLineError(new ConsoleText(message) { TextColor = foregroundColor });
        }
        public static void WriteLineErrorWithColor(this ICommandEnvironment env, string message, Color foregroundColor, Color backgroundColor)
        {
            env.WriteLineError(new ConsoleText(message) { TextColor = foregroundColor, BackColor = backgroundColor });
        }
        public static void SetWholeConsoleBackgroundColor(this ICommandEnvironment env, Color bgColor)
        {
            env.ResetAllColors();
            env.SetBackgroundColor(bgColor);
        }
    }
}