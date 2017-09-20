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
    public partial class EmailAdvancedDialogBox : Form
    {
        public AdvancedEmailSettings AdvancedSettings { get; private set; }
        public EmailAdvancedDialogBox(AdvancedEmailSettings settings)
        {
            AdvancedSettings = settings;
            InitializeComponent();
        }

        private void EmailAdvancedDialogBox_Load(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = AdvancedSettings;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
