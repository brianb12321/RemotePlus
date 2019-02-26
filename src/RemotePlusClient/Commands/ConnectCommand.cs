using RemotePlusClient.CommonUI.Connection;
using RemotePlusClient.Dialogs;
using RemotePlusClient.DialogViewModels;
using RemotePlusClient.Events;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.Commands
{
    public class ConnectCommand : IBetterCommand<MenuItem>
    {
        IConnectionManager _connectManager = null;
        IDialogManager _dialogManager = null;
        MenuItem _item = null;
        public ConnectCommand(IConnectionManager m, IDialogManager dm)
        {
            _connectManager = m;
            _dialogManager = dm;
        }
        public bool ChangeCanExecute()
        {
            try
            {
                if (_connectManager.GetState() != System.ServiceModel.CommunicationState.Opened)
                {
                    _item.Enabled = true;
                    return true;
                }
                else
                {
                    _item.Enabled = false;
                    GlobalServices.EventBus.Publish(new ClientConnectedEvent(this));
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public void CommandNodified(MenuItem item, bool canExecute)
        {
            item.Enabled = canExecute;
        }

        public void Execute(object args)
        {
            var result = _dialogManager.Show<ConnectViewModel>();
            if(result.Result == DialogResult.OK)
            {
                _connectManager.Open(new Connection()
                {
                    WholeAddress = result.ViewModel.ServerAddress
                });
                _connectManager.Register(new RemotePlusLibrary.RegisterationObject());
            }
        }

        public void UIAdded(MenuItem item)
        {
            _item = item;
        }
    }
}