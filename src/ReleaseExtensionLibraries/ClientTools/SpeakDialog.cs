using RemotePlusClient;
using RemotePlusClient.ExtensionSystem;
using RemotePlusLibrary.Extension.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientTools
{
    public partial class SpeakDialog : ThemedForm, IClientExtension
    {
        public ThemedForm ExtensionForm => this;

        public ClientExtensionDetails GeneralDetails => new ClientExtensionDetails("Speak", "1.0.0.0");

        public SpeakDialog()
        {
            InitializeComponent();
        }

        private void SpeakDialog_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            VoiceGender gender = VoiceGender.Female;
            VoiceAge age = VoiceAge.Adult;
            if(comboBox1.SelectedIndex == 0)
            {
                gender = VoiceGender.Female;
            }
            else if(comboBox1.SelectedIndex == 1)
            {
                gender = VoiceGender.Male;
            }
            else if(comboBox1.SelectedIndex == 2)
            {
                gender = VoiceGender.Neutral;
            }
            else if(comboBox1.SelectedIndex == 3)
            {
                gender = VoiceGender.NotSet;
            }
            if(comboBox2.SelectedIndex == 0)
            {
                age = VoiceAge.Adult;
            }
            else if(comboBox2.SelectedIndex == 1)
            {
                age = VoiceAge.Child;
            }
            else if(comboBox2.SelectedIndex == 2)
            {
                age = VoiceAge.NotSet;
            }
            else if(comboBox2.SelectedIndex == 3)
            {
                age = VoiceAge.Senior;
            }
            else if(comboBox2.SelectedIndex == 4)
            {
                age = VoiceAge.Teen;
            }
            MainF.Remote.Speak(textBox1.Text, gender, age);
        }
    }
}
