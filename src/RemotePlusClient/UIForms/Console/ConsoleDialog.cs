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
using BetterLogger;

namespace RemotePlusClient.UIForms.Consoles
{
    public partial class ConsoleDialog : ThemedForm
    {
        public ILogFactory Logger = null;
        public ConsoleDialog()
        {
            InitializeComponent();
        }

        private void ConsoleDialog_Load(object sender, EventArgs e)
        {
            Logger = new BaseLogFactory();
            Logger.AddLogger(new TextBoxLogger(richTextBox1));
            Logger.Log("Console opened.", LogLevel.Info);
        }

        internal void AppendText(string message)
        {
            richTextBox1.AppendText(message);
        }

        private void ConsoleDialog_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
    }
}
