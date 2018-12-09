using BetterLogger;
using RemotePlusClientCmd.ClientExtensionSystem;
using RemotePlusLibrary.Core;
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
        CommandResponse banner(CommandRequest args, CommandPipeline pipe)
        {
            ClientCmdManager.ShowBanner();
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Shows help for local commands.")]
        CommandResponse Help(CommandRequest args, CommandPipeline pipe)
        {
            string helpString = string.Empty;
            if (args.Arguments.Count == 2)
            {
                helpString = RemotePlusConsole.ShowHelpPage(LocalCommands, args.Arguments[1].ToString());
            }
            else
            {
                helpString = RemotePlusConsole.ShowHelp(LocalCommands);
            }
            Console.WriteLine(helpString);
            var response = new CommandResponse((int)CommandStatus.Success);
            response.Metadata.Add("helpText", helpString);
            return response;
        }
        [CommandHelp("Clears the console screen.")]
        CommandResponse clearScreen(CommandRequest args, CommandPipeline pipe)
        {
            Console.Clear();
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Closes the connection to the server.")]
        CommandResponse close(CommandRequest args, CommandPipeline pipe)
        {
            GlobalServices.RunningEnvironment.Close();
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Changes the console title.")]
        CommandResponse title(CommandRequest args, CommandPipeline pipe)
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
        CommandResponse load_CommandFile(CommandRequest args, CommandPipeline pipe)
        {
            try
            {
                GlobalServices.Logger.Log($"Loading extension library. {args.Arguments[1]}", LogLevel.Info);
                ClientCmdManager.ExtensionLibraries.LoadExtension(args.Arguments[1].ToString(), new ClientInitEnvironment(false));
                GlobalServices.Logger.Log("Finished.", LogLevel.Info);
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (Exception ex)
            {
                GlobalServices.Logger.Log($"Unable to load command file: {ex.Message}", LogLevel.Error);
                return new CommandResponse((int)CommandStatus.Success);
            }
        }
        [CommandHelp("loads a specified script file.")]
        public CommandResponse loadScriptFIle(CommandRequest args, CommandPipeline pipe)
        {
            bool success = ClientCmdManager.Remote.ExecuteScript(File.ReadAllText(args.Arguments[1].ToString()));
            return (success == true ? new CommandResponse((int)CommandStatus.Success) : new CommandResponse((int)CommandStatus.Fail));
        }
    }
}