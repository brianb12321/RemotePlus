using RemotePlusLibrary;
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
            progressWorker.RunWorkerAsync();
        }
        public void Start()
        {

        }
        private void ProgressWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Invoke((Action)(() => fileBrowser1.StatusMessage = "Downloading file data"));
            int num = 0;
            PopulateTreeView(() =>
            {
                fileBrowser1.CountLabel = (num++);
            });
        }
        void PopulateTreeView(Action callback)
        {
            TreeNode rootNode = null;
            try
            {
                RemoteDirectory info = remote.GetRemoteFiles(true);
                this.Invoke((Action)(() => fileBrowser1.StatusMessage = "Populating Tree View"));
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode, callback);
                this.Invoke((Action)(() => fileBrowser1.Directories.Add(rootNode)));
                fileBrowser1.StatusMessage = "Finsished";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not download file data: {ex.Message}");
                this.Invoke((Action)(() => fileBrowser1.StatusMessage = "Donwload failed."));
            }
        }

        private void GetDirectories(RemoteDirectory[] subDirs, TreeNode nodeToAdd, Action callback)
        {
            TreeNode aNode;
            RemoteDirectory[] subSubDirs;
            int number = subDirs.Length;
            foreach (RemoteDirectory subDir in subDirs)
            {
                try
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
                catch (Exception ex)
                {

                }
            }
        }
    }
}