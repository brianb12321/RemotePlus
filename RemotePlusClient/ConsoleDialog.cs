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
    public partial class ConsoleDialog : Form
    {
        public RichTextBoxLoggingMethod Logger = new RichTextBoxLoggingMethod();
        public ConsoleDialog()
        {
            InitializeComponent();
        }

        private void ConsoleDialog_Load(object sender, EventArgs e)
        {
            Logger = new RichTextBoxLoggingMethod();
            Logger.DefaultFrom = "Client";
            Logger.Output = richTextBox1;
            Logger.OverrideLogItemObjectColorValue = true;
            Logger.AddOutput("Console opened.", OutputLevel.Info);
        }

        internal void AppendText(string message)
        {
            richTextBox1.AppendText(message);
        }
    }
}
