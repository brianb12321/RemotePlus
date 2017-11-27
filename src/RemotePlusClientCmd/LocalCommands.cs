using Logging;
using RemotePlusClientCmd.ClientCommandSystem;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClientCmd
{
    partial class ClientCmdManager
    {
        [CommandHelp("Shows the banner.")]
        static CommandResponse banner(CommandRequest args, CommandPipeline pipe)
        {
            ClientCmdManager.ShowBanner();
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Shows help for local commands.")]
        static CommandResponse Help(CommandRequest args, CommandPipeline pipe)
        {
            Logger.AddOutput(RemotePlusConsole.ShowHelp(LocalCommands, args.Arguments), OutputLevel.Info);
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Clears the console screen.")]
        static CommandResponse clearScreen(CommandRequest args, CommandPipeline pipe)
        {
            Console.Clear();
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Closes the connection to the server.")]
        static CommandResponse close(CommandRequest args, CommandPipeline pipe)
        {
            Remote.Disconnect();
            channel.Close();
            Environment.Exit(0);
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Changes the console title.")]
        static CommandResponse title(CommandRequest args, CommandPipeline pipe)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 1; i < args.Arguments.Length; i++)
            {
                sb.AppendFormat("{0} ", args.Arguments[i]);
            }
            Console.Title = sb.ToString();
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Loads the specified client command library.")]
        static CommandResponse load_CommandFile(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                Logger.AddOutput($"Loading command file. {args.Arguments[1]}", OutputLevel.Info);
                ClientCommandLibraryLoader.LoadCommandLibrary(args.Arguments[1]);
                Logger.AddOutput("Finished.", OutputLevel.Info);
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (Exception ex)
            {
                Logger.AddOutput($"Unable to load command file: {ex.Message}", OutputLevel.Error);
                return new CommandResponse((int)CommandStatus.Success);
            }
        }
    }
}