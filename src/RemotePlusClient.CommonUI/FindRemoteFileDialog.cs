using RemotePlusLibrary;
using RemotePlusLibrary.FileTransfer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.CommonUI
{
    public partial class FindRemoteFileDialog : Form
    {
        public string FilePath { get; private set; }
        public int Counter
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
        IRemote remote = null;
        public FindRemoteFileDialog(IRemote r)
        {
            remote = r;
            InitializeComponent();
        }

        private void fileBrowser1_FileSelected(object sender, FileSelectedEventArgs e)
        {
            textBox1.Text = e.SelectedName;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            FilePath = Path.Combine(fileBrowser1.CurrentPath, textBox1.Text);
            Close();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            progressWorker.CancelAsync();
            Close();
        }

        private void FindRemoteFileDialog_Load(object sender, EventArgs e)
        {
            Counter = 0;
            progressWorker.DoWork += ProgressWorker_DoWork;
            //progressWorker.RunWorkerCompleted += (WSender, WE) => MessageBox.Show($"Error during work: {WE.Error?.Message}");
            progressWorker.RunWorkerAsync();
        }
        public void Start()
        {

        }
        private void ProgressWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Invoke((Action)(() => fileBrowser1.StatusMessage = "Downloading file data"));
            TreeNode rootNode = null;
            RemoteDirectory info = remote.GetRemoteFiles($@"c:\users\{Environment.UserName}", true);
            Invoke((Action)(() => fileBrowser1.StatusMessage = "Populating Tree View"));
            rootNode = new TreeNode(info.Name);
            rootNode.Tag = info;
            //foreach(RemoteDirectory dir in info.Directories)
            //{
            //    TreeNode newNode = new TreeNode(dir.Name);
            //    newNode.Tag = dir;
            //    rootNode.Nodes.Add(newNode);
            //}
            PopulateTree(rootNode);
            //GetDirectories(info.GetDirectories(), rootNode, callback);
            this.Invoke((Action)(() => fileBrowser1.Directories.Add(rootNode)));
            fileBrowser1.StatusMessage = "Finsished";
        }
        void PopulateTree(TreeNode e)
        {
            try
            {
                Counter = 0;
                RemoteDirectory dir = e.Tag as RemoteDirectory;
                var newDir = remote.GetRemoteFiles(dir.FullName, true);
                e.Tag = newDir;
                if (e.Nodes.Count == 0)
                {
                    fileBrowser1.StatusMessage = "Getting data";
                    foreach (RemoteDirectory dirs in newDir.GetDirectories())
                    {
                        Counter++;
                        TreeNode node = new TreeNode(dirs.Name);
                        node.Tag = dirs;
                        node.ImageKey = "Folder";
                        e.Nodes.Add(node);
                    }
                    fileBrowser1.StatusMessage = "Finsished";
                }
            }
            catch (FaultException ex)
            {
                MessageBox.Show($"The server could not access the directory: {ex.Message}");
            }
        }
        private void fileBrowser1_TreeVewAfterSelect(object sender, TreeViewEventArgs e)
        {
            RemoteDirectory nodeDirInfo = (RemoteDirectory)e.Node.Tag;
            PopulateTree(e.Node);
            fileBrowser1.CurrentPath = nodeDirInfo.FullName;
            fileBrowser1.FileList.Items.Clear();
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;
            if (fileBrowser1.Filter == FilterMode.Both || fileBrowser1.Filter == FilterMode.Directory)
            {
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
            }
            if (fileBrowser1.Filter == FilterMode.Both || fileBrowser1.Filter == FilterMode.File)
            {
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
            }

            fileBrowser1.FileList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

        }

        private void GetDirectories(RemoteDirectory[] subDirs, TreeNode nodeToAdd, Action callback)
        {
            TreeNode aNode;
            RemoteDirectory[] subSubDirs;
            int number = subDirs.Length;
            foreach (RemoteDirectory subDir in subDirs)
            {
                //MainF.ConsoleObj.Logger.AddOutput($"Adding {subDir.FullName}.", Logging.OutputLevel.Info);
                callback();
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subSubDirs, aNode, callback);
                }
                nodeToAdd.Nodes.Add(aNode);
            }
        }

        private void fileBrowser1_NodeAboutToBeExpanded(object sender, TreeViewCancelEventArgs e)
        {

        }
    }
}