﻿using RemotePlusClient.CommonUI.Connection;
using RemotePlusLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.UIForms.Connection
{
    public partial class ConnectDialog : Form
    {
        public bool UseProxy { get; private set; }
        ConnectAdvancedDialogBox cadb = new ConnectAdvancedDialogBox();
        public EndpointAddress Address { get; private set; }
        public RegisterationObject RegObject { get; private set; }
        public ConnectDialog()
        {
            InitializeComponent();
            RegObject = new RegisterationObject();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Address = new EndpointAddress(textBox1.Text);
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (cadb.ShowDialog() == DialogResult.OK)
            {
                RegObject = cadb.RegObject;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (ConnectionBuilderDialogBox cb = new ConnectionBuilderDialogBox())
            {
                if(cb.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = cb.NewUrl;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (SelectConfigurationDialog configDialog = new SelectConfigurationDialog())
            {
                if(configDialog.ShowDialog() == DialogResult.OK)
                {
                    Address = new EndpointAddress(configDialog.SelectedConnection.ServerAddress);
                    RegObject = configDialog.SelectedConnection.RegisterationDetails;
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Address = new EndpointAddress(textBox1.Text);
            UseProxy = true;
            Close();
        }

        private void ConnectDialog_Load(object sender, EventArgs e)
        {

        }
    }
}