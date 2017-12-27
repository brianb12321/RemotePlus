using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusLibrary.FileTransfer;

namespace RemotePlusClient.CommonUI
{
    public partial class FileBrowser : UserControl
    {

        #region Public Properties
        const string FILE_BROWSER_CATEGORY = "File Browser";
        [DisplayName("Directories")]
        [Description("The nodes that represent directories.")]
        [Category(FILE_BROWSER_CATEGORY)]
        public TreeNodeCollection Directories
        {
            get
            {
                return treeView1.Nodes;
            }
        }
        [DisplayName("Current Path")]
        [Description("The current path that is selected")]
        [Category(FILE_BROWSER_CATEGORY)]
        public string CurrentPath
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }
        [DisplayName("Status Message")]
        [Description("The current status.")]
        [Category(FILE_BROWSER_CATEGORY)]
        public string StatusMessage
        {
            get
            {
                return labelStatus.Text;
            }
            set
            {
                labelStatus.Text = value;
            }
        }
        [DisplayName("Count Label")]
        [Description("The text for the counter.")]
        [Category(FILE_BROWSER_CATEGORY)]
        public int CountLabel
        {
            get
            {
                return int.Parse(labelMin.Text);
            }
            set
            {
                labelMin.Text = value.ToString();
            }
        }
        [DisplayName("Filter")]
        [Description("Sets what to filter out of the list.")]
        [Category(FILE_BROWSER_CATEGORY)]
        public FilterMode Filter { get; set; } = FilterMode.Both;
        public TreeNode SelectedNode
        {
            get
            {
                return treeView1.SelectedNode;
            }
        }
        public ListView FileList
        {
            get
            {
                return listView1;
            }
        }
        #endregion
        #region Public Events
        public event EventHandler<FileSelectedEventArgs> FileSelected;
        public event EventHandler<TreeViewCancelEventArgs> NodeAboutToBeExpanded;
        public event EventHandler<TreeViewEventArgs> TreeVewAfterSelect;
        #endregion

        protected virtual void OnFileSelected(FileSelectedEventArgs e)
        {
            FileSelected?.Invoke(this, e);
        }
        protected virtual void OnNodeAboutToBeExpanded(TreeViewCancelEventArgs e)
        {
            NodeAboutToBeExpanded?.Invoke(this, e);
        }
        protected virtual void OnTreeVewAfterSelect(TreeViewEventArgs e)
        {
            TreeVewAfterSelect?.Invoke(this, e);
        }

        public FileBrowser()
        {
            InitializeComponent();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            OnTreeVewAfterSelect(e);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            OnTreeVewAfterSelect(new TreeViewEventArgs(e.Node));
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            OnNodeAboutToBeExpanded(e);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }
            else
            {
                var selectedItem = listView1.SelectedItems[0];
                OnFileSelected(new FileSelectedEventArgs(selectedItem.SubItems[0].Text, selectedItem.Text));
            }
        }
    }
}