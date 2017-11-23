using RemotePlusClient.ExtensionSystem;
using RemotePlusLibrary.Extension.Gui;
using RemotePlusLibrary.FileTransfer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientTools
{
    public partial class FileTransfer : ThemedForm, IClientExtension
    {
        public ThemedForm ExtensionForm => this;

        public ClientExtensionDetails GeneralDetails => new ClientExtensionDetails("FileTransfer", "1.0.0.0");

        public bool StaticPositioned => true;

        public FormPosition Position => FormPosition.Top;

        public FileTransfer()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
            }
        }

        private void FileTransfer_Load(object sender, EventArgs e)
        {

        }
    }
}
