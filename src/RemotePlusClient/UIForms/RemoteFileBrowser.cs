using BetterLogger;
using RemotePlusClient.CommonUI.Controls.FileBrowserHelpers;
using RemotePlusLibrary.Extension.Gui;
using RemotePlusLibrary.FileTransfer.BrowserClasses;
using RemotePlusLibrary.FileTransfer.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.ServiceModel;
using System.Windows.Forms;

namespace RemotePlusClient.UIForms
{
    //Credit goes to Microsoft article at https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/creating-an-explorer-style-interface-with-the-listview-and-treeview
    public partial class RemoteFileBrowser : ThemedForm
    {
        private FileAssociationSettings associations;
        FileTransfer ft = new FileTransfer(MainF.CurrentConnectionData.BaseAddress, MainF.CurrentConnectionData.Port);

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

        private void fileBrowser1_FileSelected(object sender, FileSelectedEventArgs e)
        {
            
        }

        public RemoteFileBrowser()
        {
            InitializeComponent();
        }

        private void RemoteFileBrowser_Load(object sender, EventArgs e)
        {
            associations = new FileAssociationSettings();
            if (File.Exists(FileAssociationSettings.FILE_PATH))
            {
                associations = new RemotePlusLibrary.Configuration.StandordDataAccess.ConfigurationHelper().LoadConfig<FileAssociationSettings>(FileAssociationSettings.FILE_PATH);
            }
            SetupAssociation();
            Counter = 0;
            MainF.ConsoleObj.Logger.Log("Downloading file data from server. This may take a while.", LogLevel.Info);
            progressWorker.DoWork += ProgressWorker_DoWork;
            progressWorker.RunWorkerAsync();
        }
        private void ProgressWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Invoke((Action)(() => fileBrowser1.StatusMessage = "Downloading file data"));
            TreeNode rootNode = null;
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                RemoteDrive info = (RemoteDrive)MainF.Remote.GetRemoteFiles($@"{drive.RootDirectory}", true);
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
                    RemoteDrive newDrive = (RemoteDrive)MainF.Remote.GetRemoteFiles(drive.FullName, true);
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
                    RemoteDirectory newDir = (RemoteDirectory)MainF.Remote.GetRemoteFiles(dir.FullName, true);
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
        private void fileBrowser1_NodeAboutToBeExpanded(object sender, TreeViewCancelEventArgs e)
        {
           
        }

        private string CheckAssociation(string extension)
        {
            if (fileBrowser1.FileList.SmallImageList.Images.ContainsKey(extension))
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

        private void fileBrowser1_TreeVewAfterSelect(object sender, TreeViewEventArgs e)
        {
            IDirectory nodeDirInfo = null;
            if (e.Node.GetNodeCount(true) == 0 && !(e.Node.Tag is RemoteDrive))
            {
                PopulateTree(e.Node);
            }
            if (e.Node.Tag is RemoteDrive)
            {
                e.Node.SelectedImageKey = "drive.ico";
                nodeDirInfo = (RemoteDrive)e.Node.Tag;
            }
            else
            {
                nodeDirInfo = (RemoteDirectory)e.Node.Tag;
            }
            fileBrowser1.CurrentPath = nodeDirInfo.FullName;
            fileBrowser1.FileList.Items.Clear();
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;
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

            fileBrowser1.FileList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
    }
}