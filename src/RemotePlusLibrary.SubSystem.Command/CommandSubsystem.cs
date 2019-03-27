using RemotePlusLibrary.Extension;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command
{
    public abstract class CommandSubsystem<TModule> : BaseExtensionSubsystem<ICommandSubsystem<TModule>, TModule>, ICommandSubsystem<TModule> where TModule : ICommandModule
    {
        protected CommandSubsystem(IExtensionLibraryLoader loader) : base(loader)
        {
        }
        public virtual CommandPipeline RunServerCommand(string command, CommandExecutionMode commandMode)
        {
            return RunServerCommandAsync(command, commandMode).Result;
        }
        public abstract Task<CommandPipeline> RunServerCommandAsync(string command, CommandExecutionMode commandMode);
        public abstract void Cancel();
        public CommandDelegate GetCommand(string name)
        {
            foreach (TModule cc in GetAllModules())
            {
                if (cc.HasCommand(name))
                {

                    return cc.Lookup(name);
                }
            }
            return null;
        }
        public bool HasCommand(string name)
        {
            foreach (TModule cc in GetAllModules())
            {
                if (cc.HasCommand(name))
                {
                    return true;
                }
            }
            return false;
        }
        public string ShowHelpScreen()
        {
            IReadOnlyDictionary<string, CommandDelegate> commands = AggregateAllCommandModules();
            StringBuilder helpBuilder = new StringBuilder();
            var serverData = Assembly.GetEntryAssembly().GetName();
            helpBuilder.AppendLine($"{serverData.Name} [Version: {serverData.Version}]")
                .AppendLine()
                .AppendLine();
            var padWidth = commands.Max(c => c.Key.Length) + 5;
            foreach (KeyValuePair<string, CommandDelegate> c in commands)
            {
                bool index = true;
                var behavior = GetCommandBehavior(c.Value);
                if (behavior != null)
                {
                    if (!behavior.IndexCommandInHelp)
                    {
                        index = false;
                    }
                }
                if (c.Value.Method.GetCustomAttributes(false).Length > 0)
                {
                    CommandHelpAttribute cha = GetCommandHelp(c.Value);
                    if(cha != null && index)
                    {
                        string paddedString = c.Key.PadRight(padWidth);
                        helpBuilder.Append(paddedString)
                            .AppendLine(cha.HelpMessage);
                    }
                }
                else
                {
                    if (index == true)
                    {
                        helpBuilder.AppendLine(c.Key);
                    }
                }
            }
            return helpBuilder.ToString();
        }
        public IReadOnlyDictionary<string, CommandDelegate> AggregateAllCommandModules()
        {
            Dictionary<string, CommandDelegate> commands = new Dictionary<string, CommandDelegate>();
            foreach (TModule cc in ExtensionLoader.GetAllModules<TModule>())
            {
                foreach (KeyValuePair<string, CommandDelegate> loopCommands in cc.Commands)
                {
                    commands.Add(loopCommands.Key, loopCommands.Value);
                }
            }
            return new ReadOnlyDictionary<string, CommandDelegate>(commands);
        }
        public CommandBehaviorAttribute GetCommandBehavior(CommandDelegate command)
        {
            try
            {
                object a = command.Method.GetCustomAttributes(false).Where(t => t is CommandBehaviorAttribute)
                .First();
                if (a != null)
                {
                    return (CommandBehaviorAttribute)a;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        public CommandHelpAttribute GetCommandHelp(CommandDelegate command)
        {
            try
            {
                object a = command.Method.GetCustomAttributes(false).Where(t => t is CommandHelpAttribute)
                .First();
                if (a != null)
                {
                    return (CommandHelpAttribute)a;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        public HelpPageAttribute GetHelpPage(CommandDelegate command)
        {
            try
            {
                object a = command.Method.GetCustomAttributes(false).Where(t => t is HelpPageAttribute)
                .First();
                if (a != null)
                {
                    return (HelpPageAttribute)a;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        public string ShowCommandHelpDescription(string command)
        {
            string t = "";
            IReadOnlyDictionary<string, CommandDelegate> commands = AggregateAllCommandModules();

            if (commands[command].Method.GetCustomAttributes(false).Length > 0)
            {
                foreach (object o in commands[command].Method.GetCustomAttributes(false))
                {
                    if (o is CommandHelpAttribute)
                    {
                        CommandHelpAttribute cha = (CommandHelpAttribute)o;
                        t += cha.HelpMessage;
                    }
                }
            }
            else
            {
                throw new KeyNotFoundException();
            }
            return t;
        }
        public string ShowHelpPage(CommandDelegate command)
        {
            StringBuilder sb = new StringBuilder();
            HelpPageAttribute a = GetHelpPage(command);
            if(a != null)
            {
                if (a.Source == HelpSourceType.Text)
                {
                    sb.Append(a.Details);
                }
                else if (a.Source == HelpSourceType.File)
                {
                    foreach (string lines in File.ReadAllLines("helpDocs\\" + a.Details))
                    {
                        sb.AppendLine(lines);
                    }
                }
            }
            return sb.ToString();
        }
        public string ShowHelpPage(string command)
        {
            IReadOnlyDictionary<string, CommandDelegate> commands = AggregateAllCommandModules();
            return ShowHelpPage(commands[command]);
        }
    }
}