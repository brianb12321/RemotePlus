using RemotePlusLibrary.Core.EmailService;
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
    public partial class ConfigureEmailDialogBox : Form
    {
        public EmailSettings EmailSettings { get; set; }
        public ConfigureEmailDialogBox(EmailSettings currentSettings)
        {
            EmailSettings = currentSettings;
            InitializeComponent();
        }

        private void save_Click(object sender, EventArgs e)
        {
            Save();
        }
        private void Save()
        {
            EmailSettings.SMTPHost = this.smtp.Text;
            EmailSettings.Port = (int)this.Port.Value;
            EmailSettings.Timeout = (int)this.numericUpDown1.Value;
            EmailSettings.Username = this.username.Text;
            EmailSettings.Password = password.Text;
            EmailSettings.DefaultTo = txt_defaultrecip.Text;
            EmailSettings.FromAddress = txt_defaultFrom.Text;
            DialogResult = DialogResult.OK;
        }

        private void ConfigureEmailDialogBox_Load(object sender, EventArgs e)
        {           
            RefreshEmail();
        }
        private void RefreshEmail()
        {
            this.smtp.Text = EmailSettings.SMTPHost;
            this.Port.Value = EmailSettings.Port;
            this.label6.Text = EmailSettings.EnableSSL.ToString();
            this.username.Text = EmailSettings.Username;
            this.password.Text = EmailSettings.Password;
            this.numericUpDown1.Value = EmailSettings.Timeout;
            this.txt_defaultrecip.Text = EmailSettings.DefaultTo;
            this.txt_defaultFrom.Text = EmailSettings.FromAddress;
        }

        private void saveExit_Click(object sender, EventArgs e)
        {
            Save();
            Close();
        }

        private void Close_button_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RefreshEmail();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (EmailSettings.EnableSSL == true)
            {
                EmailSettings.EnableSSL = false;
                label6.Text = EmailSettings.EnableSSL.ToString();
                label6.Refresh();
            }
            else
            {
                EmailSettings.EnableSSL = true;
                label6.Text = EmailSettings.EnableSSL.ToString();
                label6.Refresh();
            }
        }

        private void btn_advanced_Click(object sender, EventArgs e)
        {
            using (EmailAdvancedDialogBox a = new EmailAdvancedDialogBox(EmailSettings.AdvancedSettings))
            {
                if(a.ShowDialog() == DialogResult.OK)
                {
                    EmailSettings.AdvancedSettings = a.AdvancedSettings;
                }
            }
        }
    }
}
