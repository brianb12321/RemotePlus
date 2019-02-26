using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace RemotePlusClient
{
    public interface IBetterCommand<TUI> where TUI : Menu
    {
        bool ChangeCanExecute();
        void UIAdded(TUI item);
        void Execute(object args);
    }
}