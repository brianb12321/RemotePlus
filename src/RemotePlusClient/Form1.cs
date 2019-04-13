using BetterLogger;
using RemotePlusClient.CommonUI.Connection;
using RemotePlusClient.CommonUI.Requests;
using RemotePlusClient.Events;
using RemotePlusClient.Utils;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.RequestSystem;
using System;
using System.Windows.Forms;
using static System.Windows.Forms.Menu;

namespace RemotePlusClient
{
    public partial class Form1 : Form
    {
        IWindowManager _winManager = null;
        IDialogManager _dialogManager = null;
        ICommandManager<MenuItem> _commandManager = null;
        IConnectionManager _connectManager = null;
        public Form1(IDialogManager dm, ICommandManager<MenuItem> am, IConnectionManager cm)
        {
            _dialogManager = dm;
            _connectManager = cm;
            _commandManager = am;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            _winManager = new DefaultWindowManager(tc_top, tc_bottum, tc_left, null);
            IOCContainer.Provider.AddSingleton(_winManager);
            GlobalServices.Logger.AddLogger(new WinFormLogger(_winManager));
            Bootstrapper.InitCommands(IOCContainer.Provider);
            Bootstrapper.InitRequests(_connectManager);
            recursiveglobalMenuItemCreated(mainMenu1.MenuItems);
            _connectManager.ClientCallback = new ClientCallback(_connectManager, _winManager, IOCContainer.GetService<ILogFactory>(), _dialogManager);
            _winManager.Open(new Forms.LogForm(), null);
            GlobalServices.EventBus.Subscribe<ClientConnectedEvent>(args => RequestStore.Add(new SendLocalFileByteStreamRequest(_connectManager.GetRemote())));
            GlobalServices.Logger.Log("Client started.", LogLevel.Info);
        }

        private void globalOpen(object sender, EventArgs args)
        {
            MenuItem item = sender as MenuItem;
            _commandManager.ExecuteCommand(item.Text, null);
        }
        private void recursiveglobalMenuItemCreated(MenuItemCollection item)
        {
            foreach(MenuItem mi in item)
            {
                _commandManager.UIAdded(mi.Text, mi);
                if(mi.MenuItems.Count > 0)
                {
                    recursiveglobalMenuItemCreated(mi.MenuItems);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                GlobalServices.RunningEnvironment.Close();
            }
            catch { }
        }
    }
}