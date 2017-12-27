using RemotePlusClient.CommonUI;
using RemotePlusLibrary.Extension.Gui;
using RemotePlusLibrary.FileTransfer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient
{
    //Credit goes to Microsoft article at https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/creating-an-explorer-style-interface-with-the-listview-and-treeview
    public partial class RemoteFileBrowser : ThemedForm
    {
        public int CountValue
        {
            get
            {
                return fileBrowser1.CountLabel;
            }
            set
            {
                fileBrowser1.CountLabel = value;
            }
        }

        private void fileBrowser1_FileSelected(object sender, FileSelectedEventArgs e)
        {
            
        }

        public RemoteFileBrowser()
        {
            InitializeComponent();
        }

        private void RemoteFileBrowser_Load(object sender, EventArgs e)
        {
            CountValue = 0;
            MainF.ConsoleObj.Logger.AddOutput("Downloading file data from server. This may take a while.", Logging.OutputLevel.Info);
            progressWorker.DoWork += ProgressWorker_DoWork;
            progressWorker.RunWorkerAsync();
        }
        private void ProgressWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Invoke((Action)(() => fileBrowser1.StatusMessage = "Downloading file data"));
            TreeNode rootNode = null;
            RemoteDirectory info = MainF.Remote.GetRemoteFiles($@"c:\users\{Environment.UserName}", true);
            Invoke((Action)(() => fileBrowser1.StatusMessage = "Populating Tree View"));
            rootNode = new TreeNode(info.Name);
            rootNode.Tag = info;
            PopulateTree(rootNode);
            //GetDirectories(info.GetDirectories(), rootNode, callback);
            this.Invoke((Action)(() => fileBrowser1.Directories.Add(rootNode)));
            fileBrowser1.StatusMessage = "Finsished";
        }
        void PopulateTree(TreeNode e)
        {
            try
            {
                CountValue = 0;
                RemoteDirectory dir = e.Tag as RemoteDirectory;
                var newDir = MainF.Remote.GetRemoteFiles(dir.FullName, true);
                e.Tag = newDir;
                if (e.Nodes.Count == 0)
                {
                    fileBrowser1.StatusMessage = "Getting data";
                    foreach (RemoteDirectory dirs in newDir.GetDirectories())
                    {
                        CountValue++;
                        TreeNode node = new TreeNode(dirs.Name);
                        node.Tag = dirs;
                        node.ImageKey = "Folder";
                        fileBrowser1.SelectedNode.Nodes.Add(node);
                    }
                    fileBrowser1.StatusMessage = "Finsished";
                }
            }
            catch (FaultException ex)
            {
                MessageBox.Show($"The server could not access the directory: {ex.Message}");
            }
        }
        private void fileBrowser1_NodeAboutToBeExpanded(object sender, TreeViewCancelEventArgs e)
        {
           
        }
        private void fileBrowser1_TreeVewAfterSelect(object sender, TreeViewEventArgs e)
        {
            RemoteDirectory nodeDirInfo = (RemoteDirectory)e.Node.Tag;
            PopulateTree(e.Node);
            fileBrowser1.CurrentPath = nodeDirInfo.FullName;
            fileBrowser1.FileList.Items.Clear();
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;
            foreach (RemoteDirectory dir in nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                          { new ListViewItem.ListViewSubItem(item, "Directory"),
                                    new ListViewItem.ListViewSubItem(item,
                                        dir.LastAccessTime.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                fileBrowser1.FileList.Items.Add(item);
            }
            foreach (RemoteFile file in nodeDirInfo.Files)
            {
                item = new ListViewItem(file.Name, 1);
                subItems = new ListViewItem.ListViewSubItem[]
                          { new ListViewItem.ListViewSubItem(item, "File"),
                                    new ListViewItem.ListViewSubItem(item,
                                        file.LastAccessed.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                fileBrowser1.FileList.Items.Add(item);
            }

            fileBrowser1.FileList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

    }
}