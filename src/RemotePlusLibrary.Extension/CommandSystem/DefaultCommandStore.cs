using Ninject;
using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    public class DefaultCommandStore : ICommandClassStore
    {
        public List<ICommandClass> CommandClasses => IOCContainer.Provider.GetAll<ICommandClass>().ToList();

        public IDictionary<string, CommandDelegate> GetAllCommands()
        {
            Dictionary<string, CommandDelegate> _allCommands = new Dictionary<string, CommandDelegate>();
            foreach(ICommandClass cc in CommandClasses)
            {
                foreach(KeyValuePair<string, CommandDelegate> commands in cc.Commands)
                {
                    _allCommands.Add(commands.Key, commands.Value);
                }
            }
            return _allCommands;
        }

        public CommandDelegate GetCommand(string name)
        {
            foreach(ICommandClass cc in CommandClasses)
            {
                if(cc.HasCommand(name))
                {
                    return cc.Lookup(name);
                }
            }
            return null;
        }

        public bool HasCommand(string command)
        {
            foreach (ICommandClass cc in CommandClasses)
            {
                if (cc.HasCommand(command))
                {
                    return true;
                }
            }
            return false;
        }

        public void InitializeCommands()
        {
            CommandClasses.ForEach(cc => cc.AddCommands());
        }
    }
}