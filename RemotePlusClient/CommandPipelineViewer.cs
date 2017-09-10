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

namespace RemotePlusClient
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
            foreach(KeyValuePair<int, CommandRoutine> p in pipe)
            {
                TreeNode tn = new TreeNode($"position {p.Key}");
                tn.Tag = p.Value;
                treeView1.Nodes.Add(tn);
                treeView1.Nodes[p.Key].Nodes.Add($"Command: {p.Value.Input.GetFullCommand()}");
                treeView1.Nodes[p.Key].Nodes.Add($"Status Code: {p.Value.Output.ResponseCode}");
                treeView1.Nodes[p.Key].Nodes.Add($"Status Message: {p.Value.Output.CustomStatusMessage}");
                treeView1.Nodes[p.Key].Nodes.Add("Metadata", "Metadata");
                foreach(KeyValuePair<string, string> m in p.Value.Output.Metadata)
                {
                    treeView1.Nodes[p.Key].Nodes["Metadata"].Nodes.Add($"{m.Key}: {m.Value}");
                }
            }
        }

        private void ClearTreeView()
        {
            treeView1.Nodes.Clear();
        }
    }
}