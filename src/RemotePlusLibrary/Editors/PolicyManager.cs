using RemotePlusLibrary.AccountSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusLibrary.Editors
{
    public partial class PolicyManager : Form
    {
        public SecurityPolicyFolder Folder = null;
        public PolicyManager(SecurityPolicyFolder f)
        {
            Folder = f;
            InitializeComponent();
        }

        private void PolicyManager_Load(object sender, EventArgs e)
        {

        }
        public void InitializeDsta()
        {
            TreeNode finalNode = new TreeNode(Folder.Name);
            finalNode.Tag = Folder;
            populateFolders(finalNode, Folder);
            treeView1.Nodes.Add(finalNode);
        }
        void populateFolders(TreeNode rootNode, SecurityPolicyFolder f)
        {
            foreach(SecurityPolicyFolder lf in f.Folders)
            {
                TreeNode tn = new TreeNode(lf.Name);
                tn.Tag = lf;
                rootNode.Nodes.Add(tn);
                if (lf.Folders.Count > 1)
                {
                    populateFolders(rootNode, lf);
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null)
            {
                populateListView(e.Node);
            }
        }

        private void populateListView(TreeNode node)
        {
            listView1.Items.Clear();
            foreach(SecurityPolicy p in ((SecurityPolicyFolder)node.Tag).Policies)
            {
                ListViewItem item = new ListViewItem(new string[] { p.ShortName, p.Description});
                item.Name = p.ShortName;
                item.Tag = new PolicyManagerClasses.PolicyView(p, p.PolicyEditorType);
                listView1.Items.Add(item);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems[0] != null)
            {
                var selectedPolicyView = (((PolicyManagerClasses.PolicyView)listView1.SelectedItems[0].Tag));
                PolicyManagerClasses.IPolicyEditor db = PolicyManagerClasses.PolicyEditors.Get(selectedPolicyView.Type);
                db.ShowDialog(selectedPolicyView);
            }
        }
    }
}