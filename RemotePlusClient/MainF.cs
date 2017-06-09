using RemotePlusClient.CommandDialogs;
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

namespace RemotePlusClient
{
    public partial class MainF : Form
    {
        public static ServerConsole ServerConsoleObj = null;
        public static IRemote Remote = null;
        public static ConsoleDialog ConsoleObj = null;
        public static ClientLibraryCollection DefaultCollection { get; private set; }
        string Address { get; set; }
        static DuplexChannelFactory<IRemote> channel = null;
        public MainF()
        {
            DefaultCollection = new ClientLibraryCollection();
            InitializeComponent();
        }
        public static void Disconnect()
        {
            if(channel != null)
            {
                Remote.Disconnect();
                channel.Close();
                ConsoleObj.Logger.DefaultFrom = "Client";
                ConsoleObj.Logger.AddOutput("Closed", Logging.OutputLevel.Info);
            }
        }
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectDialog d = new ConnectDialog();
            if (d.ShowDialog() == DialogResult.OK)
            {
                Address = d.Address;
                Connect(d.RegObject);
            }
        }
        private void OpenConsole()
        {
            ConsoleObj = new ConsoleDialog();
            AddTabToConsoleTabControl("Console", ConsoleObj);
        }
        private void Connect(RegistirationObject Settings)
        {
            try
            {
                channel = new DuplexChannelFactory<IRemote>(new ClientCallback(), "DefaultEndpoint");
                channel.Endpoint.Address = new EndpointAddress(Address);
                Remote = channel.CreateChannel();
                ConsoleObj.Logger.AddOutput("Registering...", Logging.OutputLevel.Info);
                Remote.Register(Settings);
                connectMenuItem.Enabled = false;
                consoleMenuItem.Enabled = true;
                settingsMenuItem.Enabled = true;
                switchUserMenuItem.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to client. " + ex.Message, "Connection error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void AddTabToConsoleTabControl(string Name, Form c)
        {
            Random r = new Random();
            string Id = $"{c.Name} {r.Next(1, 9999).ToString()}";
            c.Name = Id;
            c.WindowState = FormWindowState.Maximized;
            c.FormBorderStyle = FormBorderStyle.None;
            c.TopLevel = false;
            c.Dock = DockStyle.Fill;
            TabPage t = new TabPage(Name)
            {
                Name = Id
            };
            t.Controls.Add(c);
            tabControl1.TabPages.Add(t);
            c.Show();
        }
        void AddTabToMainTabControl(string Name, Form c)
        {
            Random r = new Random();
            string Id = $"{c.Name} {r.Next(1, 9999).ToString()}";
            c.Name = Id;
            c.WindowState = FormWindowState.Maximized;
            c.FormBorderStyle = FormBorderStyle.None;
            c.TopLevel = false;
            c.Dock = DockStyle.Fill;
            TabPage t = new TabPage(Name)
            {
                Name = Id
            };
            t.Controls.Add(c);
            tabControl2.TabPages.Add(t);
            c.Show();
        }
        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ConsoleObj.Logger.AddOutput("Unknown error: " + e.Exception.Message, Logging.OutputLevel.Error);
        }

        private void consoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (channel.State == CommunicationState.Opened)
            {
                if (ServerConsoleObj == null)
                {
                    ServerConsoleObj = new ServerConsole();
                    AddTabToMainTabControl("Server Console", ServerConsoleObj);
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
        private void MainF_Load(object sender, EventArgs e)
        {
            OpenConsole();
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ConsoleObj.Logger.AddOutput("Unkown error: " + ((Exception)e.ExceptionObject).Message, OutputLevel.Error);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "mi_open")
            {
                if(treeView1.SelectedNode.Name == "nd_speak")
                {
                    AddTabToMainTabControl("Speak", new SpeakDialog());
                }
                else if(treeView1.SelectedNode.Name == "nd_beep")
                {
                    AddTabToMainTabControl("Beep", new BeepDialog());
                }
                else if(treeView1.SelectedNode.Name == "nd_FileTransfer")
                {
                    AddTabToMainTabControl("FileTransfer", new FileTransfer());
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
                var lib = ClientExtensionLibrary.LoadClientLibrary(ofd.FileName, (f) => MainF.ConsoleObj.Logger.AddOutput($"Form load: {f.GeneralDetails.Name}", OutputLevel.Info));
                DefaultCollection.Libraries.Add(lib.Name, lib);
                Task.Factory.StartNew(() =>
                {
                    foreach (KeyValuePair<string, IClientExtension> f2 in DefaultCollection.GetAllExtensions())
                    {
                        TreeNode tn = new TreeNode(f2.Value.GeneralDetails.Name)
                        {
                            Name = f2.Key
                        };
                        this.Invoke(new MethodInvoker(() => treeView2.Nodes.Add(tn)));
                    }
                    this.Invoke(new MethodInvoker(() => MainF.ConsoleObj.Logger.AddOutput("Extension loaded.", Logging.OutputLevel.Info)));
                });
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Name == "emi_open")
            {
                if(treeView2.SelectedNode != null)
                {
                    Form newForm = DefaultCollection.GetAllExtensions()[treeView2.SelectedNode.Name].ExtensionForm;
                    AddTabToMainTabControl(newForm.Name, newForm);
                }
                else
                {
                    MessageBox.Show("Please select a node.");
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl2.TabPages.Count > 0)
            {
                if(tabControl2.SelectedTab.Name.Contains("ServerConsole"))
                {
                    tabControl2.TabPages.Remove(tabControl2.SelectedTab);
                    ServerConsoleObj = null;
                }
                else
                {
                    tabControl2.TabPages.Remove(tabControl2.SelectedTab);
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
            if (channel.State == CommunicationState.Opened)
            {
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    Title = "Pick a script file to run."
                };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (ServerConsoleObj == null)
                    {
                        ServerConsoleObj = new ServerConsole(ofd.FileName);
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
    }
}