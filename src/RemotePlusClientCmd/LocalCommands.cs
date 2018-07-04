using BetterLogger;
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
            Logger.Log(RemotePlusConsole.ShowHelp(LocalCommands, args.Arguments.Select(f => f.ToString()).ToArray()), LogLevel.Info);
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
            Remote?.Disconnect();
            Remote?.Close();
            Proxy?.ProxyDisconnect();
            Proxy?.Close();
            Environment.Exit(0);
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Changes the console title.")]
        static CommandResponse title(CommandRequest args, CommandPipeline pipe)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 1; i < args.Arguments.Count; i++)
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
                Logger.Log($"Loading command file. {args.Arguments[1]}", LogLevel.Info);
                ClientCommandLibraryLoader.LoadCommandLibrary(args.Arguments[1].Value);
                Logger.Log("Finished.", LogLevel.Info);
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (Exception ex)
            {
                Logger.Log($"Unable to load command file: {ex.Message}", LogLevel.Error);
                return new CommandResponse((int)CommandStatus.Success);
            }
        }
        [CommandHelp("loads a specified script file.")]
        public static CommandResponse loadScriptFIle(CommandRequest args, CommandPipeline pipe)
        {
            bool success = ClientCmdManager.Remote.ExecuteScript(File.ReadAllText(args.Arguments[1].Value));
            return (success == true ? new CommandResponse((int)CommandStatus.Success) : new CommandResponse((int)CommandStatus.Fail));
        }
    }
}