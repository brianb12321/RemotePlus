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
    public partial class CommandBrowserDialog : Form
    {
        public CommandBrowserDialog()
        {
            InitializeComponent();
        }
        IEnumerable<string> GetCommandNames()
        {
            return MainF.Remote.GetCommands();
        }
        private void CommandBrowserDialog_Load(object sender, EventArgs e)
        {
            listBox1.Items.AddRange(GetCommandNames().ToArray());
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBox1.Text = MainF.Remote.GetCommandHelpDescription(listBox1.SelectedItem.ToString());
            richTextBox2.Text = MainF.Remote.GetCommandHelpPage(listBox1.SelectedItem.ToString());
        }
    }
}
