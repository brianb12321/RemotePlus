using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.Gui;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusClient.Settings;
using RemotePlusLibrary.Contracts;
using RemotePlusClient.UIForms.SettingDialogs;
using RemotePlusClient.UIForms.CommandSystem;
using BetterLogger;
using RemotePlusLibrary;

namespace RemotePlusClient.UIForms.Consoles
{
    public partial class ServerConsole : ThemedForm
    {
        public ILogFactory Logger { get; set; }
        public ConsoleSettings settings = null;
        IRemote currentClient = null;
        public bool InputEnabled { get; set; } = true;
        public void ClearConsole() => richTextBox1.Clear();
        string scriptFile;
        public ServerConsole(IRemote c)
        {
            currentClient = c;
            InitializeComponent();
        }
        public ServerConsole(IRemote c, string file)
        {
            currentClient = c;
            scriptFile = file;
            InitializeComponent();
        }
        public ServerConsole(IRemote c, bool enableInput)
        {
            currentClient = c;
            InputEnabled = enableInput;
            InitializeComponent();
            if (!InputEnabled)
            {
                HideInput();
            }
        }
        private void ServerConsole_Load(object sender, EventArgs e)
        {
            settings = new ConsoleSettings();
            #region Initialize Settings
            try
            {
                settings = GlobalServices.DataAccess.LoadConfig<ConsoleSettings>(ConsoleSettings.CONSOLE_SETTINGS_PATH);
            }
            catch (FileNotFoundException)
            {
                MainF.ConsoleObj.Logger.Log("Created new console config file.", LogLevel.Info, "ServerConsole");
                GlobalServices.DataAccess.SaveConfig(settings, ConsoleSettings.CONSOLE_SETTINGS_PATH);
            }
            #endregion Initialize Settings
            Logger = new BaseLogFactory();
            Logger.AddLogger(new TextBoxLogger(richTextBox1));
            //Logger.DefaultInfoColor = settings.DefaultInfoColor;
            //Logger.DefaultErrorColor = settings.DefaultErrorColor;
            //Logger.DefaultWarningColor = settings.DefaultWarningColor;
            //Logger.DefaultDebugColor = settings.DefaultDebugColor;
            richTextBox1.Font = (Font)TypeDescriptor.GetConverter(typeof(Font)).ConvertFromString(settings.DefaultFont);
            textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();
            acsc.AddRange(currentClient.GetCommandsAsStrings().ToArray());
            textBox1.AutoCompleteCustomSource = acsc;
            if (!string.IsNullOrEmpty(scriptFile))
            {
                RunScriptFile();
            }
        }

        private void HideInput()
        {
            if(!InputEnabled)
            {
                textBox1.Visible = false;
            }
        }

        public void RunScriptFile()
        {
            try
            {
                Task.Run(() => currentClient.ExecuteScript(File.ReadAllText(scriptFile)));
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("The file does not exist.");
            }
        }
        public void RunScriptFile(string f)
        {
            try
            {
                Task.Run(() => currentClient.ExecuteScript(File.ReadAllText(f)));
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("The file does not exist.");
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                string command = textBox1.Text;
                textBox1.Clear();
                textBox1.Enabled = false;
                var result = Task.Run(() => currentClient.RunServerCommand(command, CommandExecutionMode.Client));
                result.Wait();
                textBox1.Enabled = true;
                PostResult(result.Result);
            }
        }

        private void PostResult(CommandPipeline result)
        {
            if (ClientApp.MainWindow.BottumPages.ContainsKey(CommandPipelineViewer.NAME))
            {
                ((CommandPipelineViewer)ClientApp.MainWindow.BottumPages[CommandPipelineViewer.NAME]).UpdatePipeline(result);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        internal void AppendText(string message)
        {
            richTextBox1.AppendText($"{message}\n");
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ConsoleSettingsDialogBox csd = new ConsoleSettingsDialogBox(settings))
            {
                csd.ShowDialog();
            }
        }

        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }
    }
}
