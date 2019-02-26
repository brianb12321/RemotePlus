using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace RemotePlusClient
{
    public class MenuItemCommandManager : ICommandManager<MenuItem>
    {
        Dictionary<string, IBetterCommand<MenuItem>> _commands = new Dictionary<string, IBetterCommand<MenuItem>>();

        public void AddCommand(string name, IBetterCommand<MenuItem> command)
        {
            _commands.Add(name, command);
        }

        public void ExecuteCommand(string name, object param)
        {
            IBetterCommand<MenuItem> command = _commands[name];
            if(command.ChangeCanExecute())
            {
                command.Execute(param);
            }
            command.ChangeCanExecute();
        }

        public void UIAdded(string name, MenuItem item)
        {
            if(_commands.ContainsKey(name))
            {
                _commands[name].UIAdded(item);
            }
        }
    }
}