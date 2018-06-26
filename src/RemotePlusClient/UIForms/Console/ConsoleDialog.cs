using Logging;
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

namespace RemotePlusClient.UIForms.Consoles
{
    public partial class ConsoleDialog : ThemedForm
    {
        public RichTextBoxLoggingMethod Logger = new RichTextBoxLoggingMethod();
        public ConsoleDialog()
        {
            InitializeComponent();
        }

        private void ConsoleDialog_Load(object sender, EventArgs e)
        {
            Logger = new RichTextBoxLoggingMethod()
            {
                DefaultFrom = "Client",
                Output = richTextBox1,
                OverrideLogItemObjectColorValue = true
            };
            Logger.AddOutput("Console opened.", OutputLevel.Info);
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
