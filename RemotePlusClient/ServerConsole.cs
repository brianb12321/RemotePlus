using Logging;
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
    public partial class ServerConsole : Form
    {
        public RichTextBoxLoggingMethod Logger { get; set; }
        public ServerConsole()
        {
            InitializeComponent();
        }

        private void ServerConsole_Load(object sender, EventArgs e)
        {
            Logger = new RichTextBoxLoggingMethod();
            Logger.Output = richTextBox1;
            Logger.DefaultInfoColor = Color.White;
            Logger.OverrideLogItemObjectColorValue = true;
            textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();
            acsc.AddRange(MainF.Remote.GetCommands().ToArray());
            textBox1.AutoCompleteCustomSource = acsc;
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                MainF.Remote.RunServerCommand(textBox1.Text);
                textBox1.Clear();
            }
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        internal void AppendText(string message)
        {
            richTextBox1.AppendText($"{message}\n");
        }
    }
}
