using RemotePlusClient.CommonUI.Connection;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace RemotePlusClient.Commands
{
    public class ExitCommand : IBetterCommand<MenuItem>
    {
        public bool ChangeCanExecute()
        {
            return true;
        }

        public void Execute(object args)
        {
            GlobalServices.RunningEnvironment.Close();
        }

        public void UIAdded(MenuItem item)
        {
            
        }
    }
}