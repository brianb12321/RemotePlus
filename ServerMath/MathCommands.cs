using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusServer;

namespace ServerMath
{
    public static class MathCommands
    {
        const string MATH = "Math";
        [CommandHelp("Returns the absolute value of a number.")]
        public static CommandResponse abs(CommandRequest request, CommandPipeline pipe)
        {
            var absValue = Math.Abs(long.Parse(request.Arguments[1].Value));
            return new CommandResponse((int)CommandStatus.Success) { CustomStatusMessage = absValue.ToString() };
        }
        [CommandHelp("Returns a number raised by an exponent. a^b")]
        public static CommandResponse pow(CommandRequest request, CommandPipeline pipe)
        {
            return new CommandResponse((int)CommandStatus.Success) { CustomStatusMessage = Math.Pow(double.Parse(request.Arguments[1].Value), double.Parse(request.Arguments[2].Value)).ToString() };
        }
    }
}