using RemotePlusLibrary;
using RemotePlusLibrary.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Logging;
using System.Threading;
using RemotePlusLibrary.Extension.Gui;
using RemotePlusClient.ExtensionSystem;
using RemotePlusLibrary.Core;
using System.ServiceModel.Discovery;
using RemotePlusClient.CommonUI;

namespace RemotePlusClient
{
    public partial class MainF : ThemedForm
    {
        public Dictionary<string, ThemedForm> BottumPages;
        public Dictionary<string, ThemedForm> TopPages;
        public static ServerConsole ServerConsoleObj = null;
        public static ServiceClient Remote = null;
        public static ConsoleDialog ConsoleObj = null;
        public static ClientCallback LocalCallback = null;
        public static ProxyClient FoundServers = null;
        public static string BaseAddress { get; private set; }
        public static int Port { get; set; }
        public static ClientLibraryCollection DefaultCollection { get; private set; }
        string Address { get; set; }
        public MainF()
        {
            DefaultCollection = new ClientLibraryCollection();
            InitializeComponent();
            if (ClientApp.ClientSettings.DefaultTheme.ThemeEnabled)
            {
                InitializeTheme(ClientApp.ClientSettings.DefaultTheme);
            }
        }
        public static void Disconnect()
        {
            if(Remote != null)
            {
                Remote.Disconnect();
                Remote.Close();
                ConsoleObj.Logger.DefaultFrom = "Client";
                ConsoleObj.Logger.AddOutput("Closed", Logging.OutputLevel.Info);
            }
            else if(FoundServers != null)
            {
                FoundServers.ProxyDisconnect();
                FoundServers.Close();
            }
        }
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectDialog d = new ConnectDialog();
            if (d.ShowDialog() == DialogResult.OK)
            {
                Address = d.Address.ToString();
                BaseAddress = d.Address.Uri.Host;
                Port = d.Address.Uri.Port;
                if (d.UseProxy)
                {
                    ConnectToProxyServer();
                }
                else
                {
                    Connect(d.RegObject);
                }
            }
        }
        private void OpenConsole()
        {
            ConsoleObj = new ConsoleDialog();
            AddTabToConsoleTabControl("Console", ConsoleObj);
        }
        void EnableMenuItems()
        {
            connectMenuItem.Enabled = false;
            consoleMenuItem.Enabled = true;
            settingsMenuItem.Enabled = true;
            switchUserMenuItem.Enabled = true;
            browseFile_MenuItem.Enabled = true;
        }
        void DisableMenuItems()
        {
            connectMenuItem.Enabled = true;
            consoleMenuItem.Enabled = false;
            settingsMenuItem.Enabled = false;
            switchUserMenuItem.Enabled = false;
            browseFile_MenuItem.Enabled = false;
        }
        private void Connect(RegisterationObject Settings)
        {
            try
            {
                LocalCallback = new ClientCallback(0);
                Remote = new ServiceClient(LocalCallback, _ConnectionFactory.BuildBinding(), new EndpointAddress(Address));
                Remote.Open();
                ConsoleObj.Logger.AddOutput("Registering...", Logging.OutputLevel.Info);
                Remote.Register(Settings);
                EnableMenuItems();
                cmb_servers.Items.Add(Remote);
            }
            catch (Exception ex)
            {
                ConsoleObj.Logger.AddOutput("Error connecting to server. " + ex.Message, OutputLevel.Error);
                MessageBox.Show("Error connecting to server. " + ex.Message, "Connection error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ConnectToProxyServer()
        {
            LocalCallback = new ClientCallback(0);
            Uri proxyEndpointAddress = new Uri(Address);
            FoundServers = new ProxyClient(LocalCallback, _ConnectionFactory.BuildBinding(), new EndpointAddress(proxyEndpointAddress));
            FoundServers.Connect();
            FoundServers.ProxyRegister();
            ConsoleObj.Logger.AddOutput($"Found {FoundServers.GetServers().Count()} servers joined to the proxy server.", OutputLevel.Info);
            AddTabToSideControl("Server Explorer", new ServerExplorer());
            string[] servers = FoundServers.GetServers().Select(g => g.ToString()).ToArray();
            cmb_servers.Items.AddRange(servers);
            connectMenuItem.Enabled = false;
        }
        public void AddTabToConsoleTabControl(string Name, ThemedForm c)
        {
            string Id = $"{Name}";
            c.Name = Id;
            c.WindowState = FormWindowState.Maximized;
            c.FormBorderStyle = FormBorderStyle.None;
            c.TopLevel = false;
            c.Dock = DockStyle.Fill;
            TabPage t = new TabPage(Name)
            {
                Name = Id,
            };
            BottumPages.Add(Id, c);
            t.Controls.Add(c);
            tabControl1.TabPages.Add(t);
            c.ShowAndInitializeTheme(ClientApp.ClientSettings.DefaultTheme);
        }
        public void AddTabToMainTabControl(string Name, ThemedForm c)
        {
            string Id = $"{Name}";
            c.Name = Id;
            c.WindowState = FormWindowState.Maximized;
            c.FormBorderStyle = FormBorderStyle.None;
            c.TopLevel = false;
            c.Dock = DockStyle.Fill;
            TabPage t = new TabPage(Name)
            {
                Name = Id
            };
            TopPages.Add(Id, c);
            t.Controls.Add(c);
            tabControl2.TabPages.Add(t);
            c.ShowAndInitializeTheme(ClientApp.ClientSettings.DefaultTheme);
        }
        public void AddTabToSideControl(string Name, ThemedForm c)
        {
            string Id = $"{Name}";
            c.Name = Id;
            c.WindowState = FormWindowState.Maximized;
            c.FormBorderStyle = FormBorderStyle.None;
            c.TopLevel = false;
            c.Dock = DockStyle.Fill;
            TabPage t = new TabPage(Name)
            {
                Name = Id
            };
            TopPages.Add(Id, c);
            t.Controls.Add(c);
            emi_Left.TabPages.Add(t);
            c.ShowAndInitializeTheme(ClientApp.ClientSettings.DefaultTheme);
        }
        void MoveFormDown(string name)
        {
            
        }
        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
#if DEBUG
            ConsoleObj.Logger.AddOutput("Unknown error: " + e.Exception.ToString(), Logging.OutputLevel.Error);
#else
            ConsoleObj.Logger.AddOutput("Unknown error: " + e.Exception.Message, Logging.OutputLevel.Error);
#endif
        }

        private void consoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenConsole(Remote, FormPosition.Top, true);
        }
        public void OpenConsole(IRemote client, FormPosition position, bool enableInput)
        {
            if (Remote == null && FoundServers != null)
            {
                if (ServerConsoleObj == null)
                {
                    ServerConsoleObj = new ServerConsole(client, enableInput);
                    if (position == FormPosition.Top)
                    {
                        AddTabToMainTabControl($"Server Console ()", ServerConsoleObj);
                    }
                    else if (position == FormPosition.Bottum)
                    {
                        AddTabToConsoleTabControl($"Server Console", ServerConsoleObj);
                    }
                }
                else
                {
                    MessageBox.Show("You can't have another console seassion.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (Remote.State == CommunicationState.Opened)
                {
                    if (ServerConsoleObj == null)
                    {
                        ServerConsoleObj = new ServerConsole(client, enableInput);
                        if (position == FormPosition.Top)
                        {
                            AddTabToMainTabControl("Server Console", ServerConsoleObj);
                        }
                        else if (position == FormPosition.Bottum)
                        {
                            AddTabToConsoleTabControl("Server Console", ServerConsoleObj);
                        }
                    }
                    else
                    {
                        MessageBox.Show("You can't have another console seassion.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please connect to a server to open console.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public void CloseBottumConsole()
        {
            if (!tabControl1.TabPages.ContainsKey("ServerConsole"))
            {
                RemoveBottumExtension(tabControl1.TabPages["ServerConsole"]);
                ServerConsoleObj = null;
            }
        }
        private void MainF_Load(object sender, EventArgs e)
        {
            TopPages = new Dictionary<string, ThemedForm>();
            BottumPages = new Dictionary<string, ThemedForm>();
            OpenConsole();
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
#if DEBUG
            if (e.ExceptionObject is FaultException<ServerFault>)
            {
                var exception = (FaultException<ServerFault>)e.ExceptionObject;
                ConsoleObj.Logger.AddOutput("Unkown error: " + exception.ToString() + Environment.NewLine + exception.Detail.ToString(), OutputLevel.Error);
            }
            else
            {
                ConsoleObj.Logger.AddOutput("Unkown error: " + ((Exception)e.ExceptionObject).ToString(), OutputLevel.Error);
            }
#else
            if (e.ExceptionObject is FaultException<ServerFault>)
            {
                var fault = e.ExceptionObject as FaultException<ServerFault>;
                ConsoleObj.Logger.AddOutput("Unkown error: " + fault.Message, OutputLevel.Error);
            }
            else
            {
                ConsoleObj.Logger.AddOutput("Unkown error: " + ((Exception)e.ExceptionObject).Message, OutputLevel.Error);
            }
#endif
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "mi_open")
            {
                var collection = DefaultCollection.GetAllExtensions();
                var extension = collection[treeView1.SelectedNode.Name];
                switch(extension.Position)
                {
                    case FormPosition.Top:
                        AddTabToMainTabControl(extension.GeneralDetails.Name, extension.ExtensionForm);
                        break;
                    case FormPosition.Bottum:
                        AddTabToConsoleTabControl(extension.GeneralDetails.Name, extension.ExtensionForm);
                        break;
                }
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerSettingsDialog ssd = new ServerSettingsDialog();
            ssd.ShowDialog();
        }

        private void MainF_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Disconnect();
            }
            catch
            {
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Choose extension library",
                Filter = "Extension type (*.dll)|*.dll"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ClientInitEnvironment env = new ClientInitEnvironment((ConsoleObj.Logger.errorcount > 0) ? true : false);
                var lib = ClientExtensionLibrary.LoadClientLibrary(ofd.FileName,
                    (f) => MainF.ConsoleObj.Logger.AddOutput($"Form load: {f.GeneralDetails.Name}", OutputLevel.Info),
                    (m, o) => ConsoleObj.Logger.AddOutput(new UILogItem(o, m, "Extension Loader")),
                    env);
                DefaultCollection.Libraries.Add(lib.Name, lib);
                Task.Factory.StartNew(() =>
                {
                    foreach (KeyValuePair<string, IClientExtension> f2 in DefaultCollection.GetAllExtensions())
                    {
                        TreeNode tn = new TreeNode(f2.Value.GeneralDetails.Name)
                        {
                            Name = f2.Key,
                            Tag = f2.Value
                        };
                        this.Invoke(new MethodInvoker(() => treeView1.Nodes.Add(tn)));
                    }
                    this.Invoke(new MethodInvoker(() => MainF.ConsoleObj.Logger.AddOutput("Extension loaded.", Logging.OutputLevel.Info)));
                });
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #region Page Control Methods
        void RemoveTopExtension(TabPage page)
        {
            tabControl2.TabPages.Remove(page);
            TopPages.Remove(page.Name);
        }
        void RemoveBottumExtension(TabPage page)
        {
            tabControl1.TabPages.Remove(page);
            BottumPages.Remove(page.Name);
        }
        #endregion

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl2.TabPages.Count > 0)
            {
                if(tabControl2.SelectedTab.Name == "Server Console")
                {
                    RemoveTopExtension(tabControl2.SelectedTab);
                    ServerConsoleObj = null;
                }
                else
                {
                    RemoveTopExtension(tabControl2.SelectedTab);
                }
            }
            else
            {
                MessageBox.Show("There are no tabs to close.");
            }
        }

        private void getServerExtensionNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(ExtensionDetails s in Remote.GetExtensionNames())
            {
                ConsoleObj.Logger.AddOutput("Extension name: " + s.Name, Logging.OutputLevel.Info);
            }
        }

        private void getExtensionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExtensionDetailsDialog edd = new ExtensionDetailsDialog();
            edd.ShowDialog();
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            if (Remote.State == CommunicationState.Opened)
            {
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    Title = "Pick a script file to run."
                };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (ServerConsoleObj == null)
                    {
                        ServerConsoleObj = new ServerConsole(Remote, ofd.FileName);
                        AddTabToMainTabControl("Server Console", ServerConsoleObj);
                    }
                    else
                    {
                        ServerConsoleObj.RunScriptFile(ofd.FileName);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please connect to a server to open console.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void switchUserMenuItem_Click(object sender, EventArgs e)
        {
            Remote.SwitchUser();
        }

        private void showRequests_MenuItem(object sender, EventArgs e)
        {
            using (ViewRequestDialogBox rdb = new ViewRequestDialogBox())
            {
                rdb.ShowDialog();
            }
        }

        private void command_browse_menuItem_Click(object sender, EventArgs e)
        {
            using (CommandBrowserDialog cbd = new CommandBrowserDialog())
            {
                cbd.ShowDialog();
            }
        }


        private void CloseAll1()
        {
            foreach(TabPage c in tabControl1.TabPages)
            {
                BottumPages.Remove(c.Name);
                tabControl1.TabPages.Remove(c);
            }
        }
        private void CloseAll2()
        {
            foreach (TabPage c in tabControl2.TabPages)
            {
                TopPages.Remove(c.Name);
                tabControl2.TabPages.Remove(c);
            }
        }

        private void browseFile_MenuItem_Click(object sender, EventArgs e)
        {
            AddTabToMainTabControl("Remote File Browser", new RemoteFileBrowser());
        }

        private void configure_menuItem_Click(object sender, EventArgs e)
        {
            using (CommonUI.EmailGui.ConfigureEmailDialogBox emailConfig = new CommonUI.EmailGui.ConfigureEmailDialogBox(Remote.GetServerEmailSettings()))
            {
                if(emailConfig.ShowDialog() == DialogResult.OK)
                {
                    Remote.UpdateServerEmailSettings(emailConfig.EmailSettings);
                }
            }
        }

        private void sendEmail_menuItem_Click(object sender, EventArgs e)
        {
            AddTabToMainTabControl("Send email", new CommonUI.EmailGui.SendEmailForm(Remote));
        }

        private void mi_pipeLineBrowser_Click(object sender, EventArgs e)
        {
            AddTabToConsoleTabControl(CommandPipelineViewer.NAME, new CommandPipelineViewer());
        }

        private void mi_closeConsoleArea_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count > 0)
            {
                if (tabControl1.SelectedTab.Name == "Console")
                {
                    MessageBox.Show("Cannot remove console.", "RemotePlusClient", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (tabControl1.SelectedTab.Name == "Server Console")
                    {
                        CloseBottumConsole();
                    }
                    else
                    {
                        RemoveBottumExtension(tabControl1.SelectedTab);
                    }
                }
            }
            else // Never should happen
            {
                MessageBox.Show("There are no tabs to close. If you see this message, there is a bug in the program.");
            }
        }

        private void cms_extensionFormTop_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch(e.ClickedItem.Name)
#pragma warning disable CS1522 // Empty switch block
            {
#pragma warning restore CS1522 // Empty switch block
                              //TODO: Add functionality.
            }
        }

        private void mi_openScriptingEnvironment_Click(object sender, EventArgs e)
        {
            AddTabToMainTabControl(ScriptingEditor.NAME, new ScriptingEditor(Remote));
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MainF_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }
    }
}