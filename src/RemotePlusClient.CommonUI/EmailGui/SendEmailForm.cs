using RemotePlusLibrary;
using RemotePlusLibrary.Core.EmailService;
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

namespace RemotePlusClient.CommonUI.EmailGui
{
    public partial class SendEmailForm : ThemedForm
    {
        IRemote r;
        public SendEmailForm(IRemote remote)
        {
            r = remote;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            r.SendEmail(Tbox_to.Text, TBox_subject.Text, RTBox_message.Text);
        }

        private void SendEmailForm_Load(object sender, EventArgs e)
        {
        }
    }
}
