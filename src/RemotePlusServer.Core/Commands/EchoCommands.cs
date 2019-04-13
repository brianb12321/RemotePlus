using System;
using System.Drawing;
using System.Linq;
using NDesk.Options;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusServer.Core.ExtensionSystem;

namespace RemotePlusServer.Core.Commands
{
    [ExtensionModule]
    public class EchoCommands : ServerCommandClass
    {
        [CommandHelp("Prints the message to the screen.")]
        public CommandResponse echo(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            string stringToPrint = string.Empty;
            string tBgColor = string.Empty;
            string tFgColor = string.Empty;
            bool help = false;
            OptionSet options = new OptionSet()
                .Add("backgroundColor|bc=", "The background color to send to the client.", v => tBgColor = v)
                .Add("foregroundColor|fc=", "The foreground color to send to the client.", v => tFgColor = v)
                .Add("help|?", "Displays the help screen.", v => help = true);
            string[] text = options.Parse(args.Arguments.Select(a => a.ToString())).ToArray();
            if (help)
            {
                options.WriteOptionDescriptions(currentEnvironment.Out);
                return new CommandResponse((int)CommandStatus.Success);
            }
            if (args.HasLastCommand)
            {
                stringToPrint = args.LastCommand + Environment.NewLine;
            }
            else
            {
                var list = text.ToList();
                list.RemoveAt(0);
                list.ForEach(t => stringToPrint += t + Environment.NewLine);
            }
            Color bgColor = Color.Empty;
            Color fgColor = Color.Empty;
            if (!string.IsNullOrWhiteSpace(tBgColor)) bgColor = ColorHelper.FromRgbArray(tBgColor.Split(','));
            if (!string.IsNullOrWhiteSpace(tFgColor)) fgColor = ColorHelper.FromRgbArray(tFgColor.Split(','));
            if(bgColor == Color.Empty && fgColor != Color.Empty)  currentEnvironment.WriteLineWithColor(stringToPrint, fgColor);
            else if (fgColor == Color.Empty && bgColor != Color.Empty) currentEnvironment.WriteLineWithColor(stringToPrint, Color.Empty, bgColor);
            else if (fgColor == Color.Empty && bgColor == Color.Empty) currentEnvironment.WriteLine(stringToPrint);
            else currentEnvironment.WriteLineWithColor(stringToPrint, fgColor, bgColor);
            return new CommandResponse((int)CommandStatus.Success)
            {
                ReturnData = stringToPrint
            };
        }

        public override void InitializeServices(IServiceCollection services)
        {
            Commands.Add("echo", echo);
        }
    }
}