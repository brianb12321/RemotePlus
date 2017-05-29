using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusLibrary.Extension;
using RemotePlusClient;

namespace ReleaseExtensions
{
    public partial class CalculatorForm : Form, IClientExtension
    {
        public CalculatorForm()
        {
            InitializeComponent();
        }

        public Form ExtensionForm => this;

        public ClientExtensionDetails GeneralDetails => new ClientExtensionDetails("CalculatorForm", "1.0.0.0");

        private void button1_Click(object sender, EventArgs e)
        {
            ExtensionExecutionContext context = new ExtensionExecutionContext(CallType.GUI);
            context.ClientExtension = GeneralDetails;
            var os = MainF.Remote.RunExtension("CalculatorExtension", context, comboBox1.SelectedText, textBox1.Text, textBox2.Text);
            if (os.Success)
            {
                label4.Text = (string)os.Data;
            }
            else
            {
                MessageBox.Show($"CalculatorExtension ran into a problem. Problem: {os.Data}");
            }
        }
    }
}
