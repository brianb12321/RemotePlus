using RemotePlusLibrary.Extension.Gui;
using RemotePlusLibrary.Scripting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.UIForms.Scripting
{
    public partial class ScriptObjectViewer : ThemedForm
    {
        ScriptGlobalInformation[] info = null;
        public ScriptObjectViewer(ScriptGlobalInformation[] i)
        {
            info = i;
            InitializeComponent();
        }

        private void ScriptObjectViewer_Load(object sender, EventArgs e)
        {
            foreach (ScriptGlobalInformation root in info)
            {
                foreach(ScriptGlobalInformation method in root.Members.Where(m => m.Type == ScriptGlobalType.Function))
                {
                    treeView1.Nodes.Add(method.Name);
                }
                PopulateTreeView(root);
            }
        }
        void PopulateTreeView(ScriptGlobalInformation i)
        {
            foreach (ScriptGlobalInformation i2 in i.Members)
            {
                TreeNode node = new TreeNode(i2.Name);
                treeView1.Nodes.Add(node);
                if(i2.Members.Count > 0)
                {
                    PopulateTreeView(i2);
                }
            }
        }
    }
}
