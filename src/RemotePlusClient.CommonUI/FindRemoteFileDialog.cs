using RemotePlusClient.CommonUI.Controls.FileBrowserHelpers;
using RemotePlusLibrary;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.FileTransfer;
using RemotePlusLibrary.FileTransfer.BrowserClasses;
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
        public FilterMode Filter { get; private set; }
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
        public FileAssociationSettings associations = null;
        IRemote remote = null;
        public FindRemoteFileDialog(FilterMode f, IRemote r, string baseURL, int p)
        {
            InitializeComponent();
            remote = r;
            port = p;
            _base = baseURL;
            fileBrowser1.BaseURL = _base;
            fileBrowser1.Port = port;
            Filter = f;
            fileBrowser1.Filter = f;
        }

        private void fileBrowser1_FileSelected(object sender, FileSelectedEventArgs e)
        {
            textBox1.Text = e.SelectedName;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) && (fileBrowser1.Filter != FilterMode.Directory || fileBrowser1.Filter != FilterMode.Both))
            {
                MessageBox.Show("You cannot select a directory in this context. Please refer to the extension documentation of more details.", "RemoteFileBrowser", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                FilePath = Path.Combine(fileBrowser1.CurrentPath, textBox1.Text);
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            progressWorker.CancelAsync();
            Close();
        }

        private void FindRemoteFileDialog_Load(object sender, EventArgs e)
        {
            associations = new FileAssociationSettings();
            associations.Load();
            SetupAssociation();
            Counter = 0;
            progressWorker.DoWork += ProgressWorker_DoWork;
            //progressWorker.RunWorkerCompleted += (WSender, WE) => MessageBox.Show($"Error during work: {WE.Error?.Message}");
            progressWorker.RunWorkerAsync();
        }
        private void ProgressWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Invoke((Action)(() => fileBrowser1.StatusMessage = "Downloading file data"));
            TreeNode rootNode = null;
            foreach(DriveInfo drive in DriveInfo.GetDrives())
            {
                RemoteDrive info = (RemoteDrive)remote.GetRemoteFiles($@"{drive.RootDirectory}", true);
                Invoke((Action)(() => fileBrowser1.StatusMessage = $"Populating Tree View for {drive.Name}"));
                rootNode = new TreeNode(drive.Name) { ImageKey = "drive.ico", Tag = info };
                //foreach(RemoteDirectory dir in info.Directories)
                //{
                //    TreeNode newNode = new TreeNode(dir.Name);
                //    newNode.Tag = dir;
                //    rootNode.Nodes.Add(newNode);s
                //}
                PopulateTree(rootNode);
                //GetDirectories(info.GetDirectories(), rootNode, callback);
                this.Invoke((Action)(() => fileBrowser1.Directories.Add(rootNode)));
            }
            
            fileBrowser1.StatusMessage = "Finsished";
        }
        void PopulateTree(TreeNode e)
        {
            try
            {
                Counter = 0;
                if (e.Tag is RemoteDrive)
                {
                    RemoteDrive drive = e.Tag as RemoteDrive;
                    RemoteDrive newDrive = (RemoteDrive)remote.GetRemoteFiles(drive.FullName, true);
                    e.Tag = newDrive;
                    fileBrowser1.StatusMessage = "Getting data";
                    foreach (RemoteDirectory dirs in newDrive.GetDirectories())
                    {
                        Counter++;
                        TreeNode node = new TreeNode(dirs.Name);
                        node.Tag = dirs;
                        node.ImageKey = "Folder";
                        e.Nodes.Add(node);
                    }
                }
                else
                {
                    RemoteDirectory dir = e.Tag as RemoteDirectory;
                    RemoteDirectory newDir = (RemoteDirectory)remote.GetRemoteFiles(dir.FullName, true);
                    e.Tag = newDir;
                    fileBrowser1.StatusMessage = "Getting data";
                    foreach (RemoteDirectory dirs in newDir.GetDirectories())
                    {
                        Counter++;
                        TreeNode node = new TreeNode(dirs.Name);
                        node.Tag = dirs;
                        node.ImageKey = "Folder";
                        e.Nodes.Add(node);
                    }
                }
                fileBrowser1.StatusMessage = "Finsished";
            }
            catch (FaultException ex)
            {
                MessageBox.Show($"The server could not access the directory: {ex.Message}");
            }
        }
        private void fileBrowser1_TreeVewAfterSelect(object sender, TreeViewEventArgs e)
        {
            IDirectory nodeDirInfo = null;
            textBox1.Text = "";
            // Makes sure that we don't get the same data from the server.
            if (e.Node.GetNodeCount(true) == 0 && !(e.Node.Tag is RemoteDrive))
            {
                PopulateTree(e.Node);
            }
            // Makes sure that we don't update the image key of a drive to a folder, and grabs the right data from the server.
            if (e.Node.Tag is RemoteDrive)
            {
                e.Node.SelectedImageKey = "drive.ico";
                nodeDirInfo = (RemoteDrive)e.Node.Tag;
            }
            else
            {
                nodeDirInfo = (RemoteDirectory)e.Node.Tag;
            }
            // Sets the current path to the selected path.
            fileBrowser1.CurrentPath = nodeDirInfo.FullName;
            // Clears the file list.
            fileBrowser1.FileList.Items.Clear();
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;
            // Determines whether to add directories to the file list.
            if (fileBrowser1.Filter == FilterMode.Both || fileBrowser1.Filter == FilterMode.Directory)
            {
                foreach (RemoteDirectory dir in nodeDirInfo.Directories)
                {
                    item = new ListViewItem(dir.Name, 0);
                    subItems = new ListViewItem.ListViewSubItem[]
                              { new ListViewItem.ListViewSubItem(item, "Directory"),
                                    new ListViewItem.ListViewSubItem(item,
                                        dir.LastAccessTime.ToShortDateString())};
                    item.SubItems.AddRange(subItems);
                    item.Tag = dir;
                    fileBrowser1.FileList.Items.Add(item);
                }
            }
            if (fileBrowser1.Filter == FilterMode.Both || fileBrowser1.Filter == FilterMode.File)
            {
                foreach (RemoteFile file in nodeDirInfo.Files)
                {
                    string key = CheckAssociation(Path.GetExtension(file.FullName));
                    item = new ListViewItem(file.Name, key);
                    subItems = new ListViewItem.ListViewSubItem[]
                              { new ListViewItem.ListViewSubItem(item, "File"),
                                    new ListViewItem.ListViewSubItem(item,
                                        file.LastAccessed.ToShortDateString())};
                    item.Tag = file;
                    item.SubItems.AddRange(subItems);
                    fileBrowser1.FileList.Items.Add(item);
                }
            }

            fileBrowser1.FileList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

        }
        public void AddFile(RemoteFile file)
        {
            ListViewItem item = new ListViewItem();
            ListViewItem.ListViewSubItem[] subItems;
            string key = CheckAssociation(Path.GetExtension(file.FullName));
            item = new ListViewItem(file.Name, key);
            subItems = new ListViewItem.ListViewSubItem[]
                      { new ListViewItem.ListViewSubItem(item, "File"),
                                    new ListViewItem.ListViewSubItem(item,
                                        file.LastAccessed.ToShortDateString())};
            item.SubItems.AddRange(subItems);
            fileBrowser1.FileList.Items.Add(item);
        }
        private string CheckAssociation(string extension)
        {
            if(fileBrowser1.FileList.SmallImageList.Images.ContainsKey(extension))
            {
                return extension;
            }
            else
            {
                return "file.ico";
            }
        }

        private void SetupAssociation()
        {
            foreach (KeyValuePair<string, string> assoc in associations.Associations)
            {
                if (!File.Exists(assoc.Value))
                {
                    //Icon file does not exist, so use default icon.
                }
                else
                {
                    Icon icon = new Icon(assoc.Value);
                    fileBrowser1.FileList.SmallImageList.Images.Add(assoc.Key, icon);
                }
            }
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

        private void fileBrowser1_EntryUpdated(object sender, EntryOperationEventArgs e)
        {
            if(e.Operation == FileOperation.Add)
            {
                
            }
        }
    }
}