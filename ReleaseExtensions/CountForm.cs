using RemotePlusClient;
using RemotePlusLibrary.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReleaseExtensions
{
    public partial class CountForm : Form, IClientExtension
    {
        public Form ExtensionForm => this;

        public ClientExtensionDetails Details => new ClientExtensionDetails("CountForm", "1.0.0.0");

        public CountForm()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void CountForm_Load(object sender, EventArgs e)
        {

        }

        public void Init()
        {
            
        }

        public void Update(object Data)
        {
            label1.Text = (string)Data;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var context = new ExtensionExecutionContext(CallType.GUI);
            context.ClientExtension = Details;
            MainF.Remote.RunExtension("CountExtension", context, "");
        }
    }
}
