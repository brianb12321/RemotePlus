using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace RemotePlusClient
{
    public interface ICommandManager<TUI> where TUI : Menu
    {
        void UIAdded(string name, TUI item);
        void ExecuteCommand(string name, object param);
        void AddCommand(string name, IBetterCommand<TUI> command);
    }
}