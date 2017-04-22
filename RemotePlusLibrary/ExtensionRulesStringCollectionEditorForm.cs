using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusLibrary
{
    public partial class ExtensionRulesStringCollectionEditorForm : Form
    {
        List<string> val = null;
        public List<string> Value { get { return val; } }
        public ExtensionRulesStringCollectionEditorForm(List<string> value)
        {
            val = value;
            InitializeComponent();
            textBox1.Text = string.Join("\r\n", value.ToArray());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                val = textBox1.Text.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while saving: " + ex);
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ExtensionRulesStringCollectionEditorForm_Load(object sender, EventArgs e)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
