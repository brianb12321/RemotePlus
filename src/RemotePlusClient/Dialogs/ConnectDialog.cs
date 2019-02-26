using RemotePlusClient.DialogViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.Dialogs
{
    public partial class ConnectDialog : Form, IDialogForm<ConnectViewModel>
    {
        public ConnectDialog()
        {
            InitializeComponent();
            ViewModel = new ConnectViewModel(FormName);
        }

        public ConnectViewModel ViewModel { get; private set; }
        public Form Form { get; private set; }
        public Guid FormID => Guid.NewGuid();
        public string FormName => Name;
        public object FormTag { get; set; }

        public void CloseForm()
        {
            Form.Dispose();
        }

        public DialogResult Open(object args)
        {
            ConnectDialog cd = new ConnectDialog();
            Form = cd;
            cd.ViewModel = ViewModel;
            return Form.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            ViewModel.ServerAddress = textBox1.Text;
            Close();
        }
    }
}