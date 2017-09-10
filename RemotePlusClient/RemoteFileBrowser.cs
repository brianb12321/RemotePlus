using RemotePlusLibrary.Extension.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient
{
    //Credit goes to Microsoft article at https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/creating-an-explorer-style-interface-with-the-listview-and-treeview
    public partial class RemoteFileBrowser : ThemedForm
    {
        public RemoteFileBrowser()
        {
            InitializeComponent();
        }

        private void RemoteFileBrowser_Load(object sender, EventArgs e)
        {
            MainF.ConsoleObj.Logger.AddOutput("Downloading file data from server. This may take a while.", Logging.OutputLevel.Info);
            Task.Run(() => this.Invoke(new MethodInvoker(delegate { PopulateTreeView(); })));
        }
        void PopulateTreeView()
        {
            TreeNode rootNode;
            DirectoryInfo info = MainF.Remote.GetRemoteFiles();
            if(info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
                MainF.ConsoleObj.Logger.AddOutput("Finished populating file browser.", Logging.OutputLevel.Info);
            }
            else
            {
                MainF.ConsoleObj.Logger.AddOutput("The directory does not exist.", Logging.OutputLevel.Warning);
            }
        }

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAdd)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            int number = subDirs.Length;
            foreach (DirectoryInfo subDir in subDirs)
            {
                try
                {
                    MainF.ConsoleObj.Logger.AddOutput($"Adding {subDir.FullName}.", Logging.OutputLevel.Info);
                    aNode = new TreeNode(subDir.Name, 0, 0);
                    aNode.Tag = subDir;
                    aNode.ImageKey = "folder";
                    subSubDirs = subDir.GetDirectories();
                    if (subSubDirs.Length != 0)
                    {
                        GetDirectories(subSubDirs, aNode);
                    }
                    nodeToAdd.Nodes.Add(aNode);
                }
                catch (Exception ex)
                {
                    MainF.ConsoleObj.Logger.AddOutput($"Directory population failed to be loaded: {ex.Message}", Logging.OutputLevel.Warning);
                }
            }
        }

        private void treeView1_Click(object sender, EventArgs e)
        {
            textBox1.Text = ((DirectoryInfo)treeView1.SelectedNode.Tag).FullName;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            listView1.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            textBox1.Text = nodeDirInfo.FullName;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                          {new ListViewItem.ListViewSubItem(item, "Directory"),
                   new ListViewItem.ListViewSubItem(item,
                dir.LastAccessTime.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }
            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                item = new ListViewItem(file.Name, 1);
                subItems = new ListViewItem.ListViewSubItem[]
                          { new ListViewItem.ListViewSubItem(item, "File"),
                   new ListViewItem.ListViewSubItem(item,
                file.LastAccessTime.ToShortDateString())};

                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

        }
    }
}