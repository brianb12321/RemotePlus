using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusClient.ViewModels;
using RemotePlusLibrary.Contracts;
using System.Threading;

namespace RemotePlusClient.Forms
{
    public partial class ConsoleForm : UserControl, ITabbedForm<ConsoleViewModel>
    {
        IRemote _remote;
        public ConsoleForm(IRemote r, Guid serverGuid)
        {
            InitializeComponent();
            Name += $" ({serverGuid})";
            _remote = r;
            ViewModel = new ConsoleViewModel(FormName, rtb_console, lbl_progressText, lbl_progress, progressBar1, splitContainer1);
            FormID = serverGuid;
        }

        public ConsoleViewModel ViewModel { get; }
        public UserControl Form => this;
        public Guid FormID { get; private set; }
        public string FormName => Name;
        public object FormTag { get; set; }
        FormPosition ITabbedForm<ConsoleViewModel>.Location { get; set; } = FormPosition.Up;

        public void CloseForm()
        {
            Form.Dispose();
        }

        private async void tb_input_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Enter:
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    tb_input.Enabled = false;
                    await _remote.RunServerCommandAsync(tb_input.Text, RemotePlusLibrary.Extension.CommandSystem.CommandExecutionMode.Client);
                    tb_input.Clear();
                    tb_input.Enabled = true;
                    break;
            }
        }

        private void rtb_console_TextChanged(object sender, EventArgs e)
        {
            rtb_console.SelectionStart = rtb_console.Text.Length;
            rtb_console.ScrollToCaret();
        }

        private void ConsoleForm_Load(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = true;
        }
    }
}