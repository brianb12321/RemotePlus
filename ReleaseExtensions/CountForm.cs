using RemotePlusClient;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusLibrary.Extension.ExtensionTypes;
using RemotePlusLibrary.Extension.ExtensionTypes.ExtensionDetailTypes;

namespace ReleaseExtensions
{
    public partial class CountForm : ThemedForm, IClientExtension
    {
        public ThemedForm ExtensionForm => this;

        public ClientExtensionDetails GeneralDetails => new ClientExtensionDetails("CountForm", "1.0.0.0");

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

        private void button1_Click(object sender, EventArgs e)
        {
            var context = new ExtensionExecutionContext(CallType.GUI)
            {
                ClientExtension = GeneralDetails
            };
            var s = MainF.Remote.RunExtension("CountExtension", context, "");
            foreach(int n in (List<int>)s.Data)
            {
                int z = n * 2;
                label1.Text = n.ToString();
            }
        }
    }
}
