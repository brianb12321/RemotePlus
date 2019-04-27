using RemotePlusLibrary.Extension;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core;

namespace RemotePlusLibrary.SubSystem.Command
{
    public interface ICommandSubsystem<out TModule> : IExtensionSubsystem<TModule> where TModule : ICommandModule
    {
        Task<CommandPipeline> RunServerCommandAsync(string command, CommandExecutionMode commandMode, IClientContext context);
        CommandPipeline RunServerCommand(string command, CommandExecutionMode commandMode, IClientContext context);
        void Cancel();
        CommandDelegate GetCommand(string name);
        bool HasCommand(string name);
        string ShowHelpScreen();
        IReadOnlyDictionary<string, CommandDelegate> AggregateAllCommandModules();
        CommandBehaviorAttribute GetCommandBehavior(CommandDelegate command);
        CommandHelpAttribute GetCommandHelp(CommandDelegate command);
        HelpPageAttribute GetHelpPage(CommandDelegate command);
        string ShowCommandHelpDescription(string command);
        string ShowHelpPage(CommandDelegate command);
        string ShowHelpPage(string command);
    }
}