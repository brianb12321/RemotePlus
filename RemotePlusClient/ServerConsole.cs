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
        public delegate bool QuestionDelegate(string answer);
        public QuestionDelegate QuestionEvent { get; set; }
        public bool QuestionMode { get; set; }
        public ServerConsole()
        {
            InitializeComponent();
        }

        private void ServerConsole_Load(object sender, EventArgs e)
        {

        }
        public void AppendText(string Text)
        {
            richTextBox1.AppendText(Text);
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if(QuestionMode)
                {
                    QuestionEvent(textBox1.Text);
                    textBox1.Clear();
                    QuestionMode = false;
                }
                else
                {
                    var Output = MainF.Remote.RunServerCommand(textBox1.Text);
                    textBox1.Clear();
                    if (Output != null)
                    {
                        foreach (LogItem l in Output)
                        {
                            richTextBox1.AppendText(l.ToString());
                        }
                    }
                }
            }
        }

        private void Execute(string Command)
        {
            
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
    }
}
