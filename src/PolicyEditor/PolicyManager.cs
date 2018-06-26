using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusLibrary.Security.AccountSystem;

namespace PolicyEditor
{
    public partial class PolicyManager : Form
    {
        public PolicyObject obj = null;
        public PolicyManager()
        {
            InitializeComponent();
        }

        private void PolicyManager_Load(object sender, EventArgs e)
        {
            LoadObject();
        }

        private void LoadObject()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select policy object to edit.";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                PolicyObject po = PolicyObject.Open(ofd.FileName);
                obj = po;
                InitializeData();
            }
        }

        public void InitializeData()
        {
            TreeNode finalNode = new TreeNode(obj.Policies.Name);
            finalNode.Tag = obj.Policies;
            populateFolders(finalNode, obj.Policies);
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
                ListViewItem item = new ListViewItem(new string[] { p.ShortName, p.ShortDescription});
                item.Name = p.ShortName;
                item.Tag = new PolicyView(p, p.PolicyEditorType);
                listView1.Items.Add(item);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems[0] != null)
            {
                var selectedPolicyView = (((PolicyView)listView1.SelectedItems[0].Tag));
                IPolicyEditor db = PolicyEditors.Get(selectedPolicyView.Type);
                db.ShowDialog(selectedPolicyView);
            }
        }

        private void mi_save_Click(object sender, EventArgs e)
        {
            obj.Save();
        }

        private void mi_load_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            listView1.Items.Clear();
            LoadObject();
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {

        }
    }
}