using BetterLogger;
using Ninject;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static RemotePlusClientCmd.ClientCmdManager;

namespace RemotePlusClientCmd.ClientExtensionSystem
{
    public class ClientCmdCommands : StandordClientCommandClass
    {
        ICommandSubsystem<IClientCmdModule> _commandSubsystem;
        [CommandHelp("Shows the banner.")]
        CommandResponse banner(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            ShowBanner();
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Shows help for local commands.")]
        CommandResponse Help(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            string helpString = string.Empty;
            if (args.Arguments.Count == 2)
            {
                helpString = _commandSubsystem.ShowHelpPage(args.Arguments[1].ToString());
            }
            else
            {
                helpString = _commandSubsystem.ShowHelpScreen();
            }
            Console.WriteLine(helpString);
            var response = new CommandResponse((int)CommandStatus.Success);
            response.Metadata.Add("helpText", helpString);
            return response;
        }
        [CommandHelp("Clears the console screen.")]
        CommandResponse clearScreen(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            Console.Clear();
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Closes the connection to the server.")]
        CommandResponse close(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            GlobalServices.RunningEnvironment.Close();
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Changes the console title.")]
        CommandResponse title(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < args.Arguments.Count; i++)
            {
                sb.AppendFormat("{0} ", args.Arguments[i]);
            }
            Console.Title = sb.ToString();
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Loads the specified client command library.")]
        CommandResponse load_CommandFile(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            try
            {
                GlobalServices.Logger.Log($"Loading extension library. {args.Arguments[1]}", LogLevel.Info);
                ClientCmdManager.ExtensionLoader.LoadFromAssembly(Assembly.LoadFile(args.Arguments[1].ToString()));
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
        public CommandResponse loadScriptFIle(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            if (ClientCmdManager.Proxy != null) ClientCmdManager.Proxy.ExecuteProxyScript(File.ReadAllText(args.Arguments[1].ToString()));
            else ClientCmdManager.Remote.ExecuteScript(File.ReadAllText(args.Arguments[1].ToString()));
            return new CommandResponse((int)CommandStatus.Success);
        }
        public override void InitializeServices(IKernel kernel)
        {
            _commandSubsystem = kernel.Get<ICommandSubsystem<IClientCmdModule>>();
            Commands.Add("#banner", banner);
            Commands.Add("#help", Help);
            Commands.Add("#clear", clearScreen);
            Commands.Add("#close", close);
            Commands.Add("#title", title);
            Commands.Add("#load-commandFile", load_CommandFile);
            Commands.Add("#execute-script", loadScriptFIle);
        }
    }
}