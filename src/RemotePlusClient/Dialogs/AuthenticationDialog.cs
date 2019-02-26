using RemotePlusLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Security.Authentication;
using RemotePlusClient.ViewModels;

namespace RemotePlusClient.Dialogs
{
    public partial class AuthenticationDialog : Form, IDialogForm<AuthenticationViewModel>
    {
        public AuthenticationRequest Request { get; private set; }
        public AuthenticationViewModel ViewModel { get; set; }
        public Form Form { get; set; }
        public Guid FormID => Guid.NewGuid();
        public string FormName => Name;
        public object FormTag { get; set; }

        public AuthenticationDialog()
        {
            ViewModel = new AuthenticationViewModel(FormName);
            InitializeComponent();
        }
        void CheckSeverity()
        {
            switch(Request.Severity)
            {
                case AuthenticationSeverity.Danger:
                    panel1.BackColor = Color.Red;
                    label4.Text = "The server is requesting authorization for a dangerous operation.";
                    break;
                case AuthenticationSeverity.Normal:
                    panel1.BackColor = Color.Blue;
                    label4.Text = "The server is requesting authorization.";
                    break;
                case AuthenticationSeverity.Risk:
                    panel1.BackColor = Color.Yellow;
                    label4.Text = "The server is requesting authorization for a risky operation.";
                    break;
            }
        }
        private void btn_login_Click(object sender, EventArgs e)
        {
            ViewModel.Credentails = new UserCredentials(textBox1.Text, textBox2.Text);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void AuthenticationDialog_Load(object sender, EventArgs e)
        {
            CheckSeverity();
            richTextBox1.AppendText(Request.Reason);
        }

        public DialogResult Open(object args)
        {
            if (args is AuthenticationRequest)
            {
                Form = new AuthenticationDialog()
                {
                    Request = args as AuthenticationRequest
                };
                ViewModel = ((AuthenticationDialog)Form).ViewModel;
                return Form.ShowDialog();
            }
            else throw new ArgumentException("Authentication requires authentication request.");
        }

        public void CloseForm()
        {
            Form.Dispose();
        }
    }
}