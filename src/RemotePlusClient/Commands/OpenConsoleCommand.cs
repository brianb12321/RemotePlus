using RemotePlusClient.CommonUI.Connection;
using RemotePlusClient.Events;
using RemotePlusClient.Forms;
using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.Commands
{
    public class OpenConsoleCommand : IBetterCommand<MenuItem>
    {
        IWindowManager _winManager = null;
        IConnectionManager _connManager = null;
        MenuItem _item = null;
        public OpenConsoleCommand(IWindowManager wm, IConnectionManager cm)
        {
            _winManager = wm;
            _connManager = cm;
        }
        public bool ChangeCanExecute()
        {
            if (_connManager.GetState() == System.ServiceModel.CommunicationState.Opened)
            {
                return true;
            }
            else
            {
                _item.Enabled = false;
                return false;
            }
        }

        public void Execute(object args)
        {
            var remote = _connManager.GetRemote();
            _winManager.Open(new ConsoleForm(remote, remote.GetSelectedServerGuid()), null);
        }

        public void UIAdded(MenuItem item)
        {
            item.Enabled = false;
            _item = item;
            GlobalServices.EventBus.Subscribe<ClientConnectedEvent>(e => item.Enabled = true);
        }
    }
}