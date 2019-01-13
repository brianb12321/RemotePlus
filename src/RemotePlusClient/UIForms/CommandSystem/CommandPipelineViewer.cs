using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.UIForms.CommandSystem
{
    public partial class CommandPipelineViewer : ThemedForm
    {
        public const string NAME = "Pipeline Browser"; 
        public CommandPipelineViewer()
        {
            InitializeComponent();
        }

        private void CommandPipelineViewer_Load(object sender, EventArgs e)
        {

        }
        public void UpdatePipeline(CommandPipeline pipe)
        {
            ClearTreeView();
            foreach(CommandRoutine p in pipe)
            {
                int position = pipe.IndexOf(p);
                TreeNode tn = new TreeNode($"position {position}");
                tn.Tag = position;
                treeView1.Nodes.Add(tn);
                treeView1.Nodes[position].Nodes.Add($"Command: {p.Input.GetFullCommand()}");
                treeView1.Nodes[position].Nodes.Add($"Status Code: {p.Output.ResponseCode}");
                treeView1.Nodes[position].Nodes.Add($"Return Data: {p.Output.ReturnData}");
                treeView1.Nodes[position].Nodes.Add("Metadata", "Metadata");
                foreach(KeyValuePair<string, string> m in p.Output.Metadata)
                {
                    treeView1.Nodes[position].Nodes["Metadata"].Nodes.Add($"{m.Key}: {m.Value}");
                }
            }
        }

        private void ClearTreeView()
        {
            treeView1.Nodes.Clear();
        }
    }
}