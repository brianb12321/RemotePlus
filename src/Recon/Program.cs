using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using System.Windows.Forms;
using Logging;
using RemotePlusClient.CommonUI;
using RemotePlusLibrary;
using RemotePlusLibrary.Core;

namespace Recon
{
    static class ReconManager
    {
        public static NotifyIcon Icon;
        public static ContextMenuStrip Menu;
        public static ServiceClient Client;
        public static CMDLogging Logger = new CMDLogging();
        public static ConsoleForm StaticConsole;
        public static ExecutionState State = ExecutionState.Idle;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Logger.DefaultFrom = "Recon";
            Application.SetCompatibleTextRenderingDefault(false);
            StaticConsole = new ConsoleForm();
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => Application_ThreadException(sender, new System.Threading.ThreadExceptionEventArgs((Exception)e.ExceptionObject));
            Console.SetOut(new MultiTextWriter(new ControlWriter(StaticConsole.Console), Console.Out));
            RequestStore.Init();
            DefaultKnownTypeManager.LoadDefaultTypes();
            Application.EnableVisualStyles();
            SetupTaskTray();
            Application.Run();
        }

        private static void SetupTaskTray()
        {
            Icon = new NotifyIcon();
            Icon.Icon = Recon.Properties.Resources.remote;
            Icon.Visible = true;
            Icon.Text = "Remote Control Monitor";
            Menu = new ContextMenuStrip();
            Menu.Items.Add(new ToolStripMenuItem("Exit and disconnect", null, Close));
            Menu.Items.Add(new ToolStripMenuItem("Connect", null, connect) { Name = "Connect"});
            Menu.Items.Add(new ToolStripMenuItem("Execute server script", null, runScript) { Enabled = false, Name = "ExecuteScript" });
            Menu.Items.Add("Show Console", null, showConsole);
            Icon.ContextMenuStrip = Menu;
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            //Logger.AddOutput($"Thread Error: {e.Exception.Message}", OutputLevel.Error);
            Icon.ShowBalloonTip(5000, "Error", "There was an unkown error. Please check the console for more details", ToolTipIcon.Error);
        }

        private static void runScript(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a script file to run on the server.";
            ofd.Filter = "Script file (*.txt, *.lua)|*.txt;*.lua";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                Menu.Items["ExecuteScript"].Enabled = false;
                BackgroundWorker bg = new BackgroundWorker();
                bg.DoWork += (sender2, e2) =>
                {
                    ChangeState(ExecutionState.Running);
                    e2.Result = Client.ExecuteScript(File.ReadAllText(ofd.FileName));
                };
                bg.RunWorkerCompleted += (sender3, e3) =>
                {
                    if ((bool)e3.Result)
                    {
                        Icon.ShowBalloonTip(5000, "Script", "Script executed successfully", ToolTipIcon.Info);
                    }
                    else
                    {
                        Icon.ShowBalloonTip(5000, "Script", "Script returned with failure code", ToolTipIcon.Warning);
                    }
                    ChangeState(ExecutionState.Idle);
                    Menu.Items["ExecuteScript"].Enabled = true;
                };
                bg.RunWorkerAsync();
            }
        }
        static void ChangeState(ExecutionState state)
        {
            State = state;
            switch (state)
            {
                case ExecutionState.Running:
                    Icon.Text = "Remote Control Monitor. Running a task.";
                    break;
                case ExecutionState.Idle:
                    Icon.Text = "Remote Control Monitor";
                    break;
            }
        }

        static void Close(object sender, EventArgs e)
        {
            if (Client != null)
            {
                if (Client.State == CommunicationState.Opened)
                {
                    Client.Disconnect();
                    Client.Close();
                }
            }
            Icon.Dispose();
            Application.Exit();
        }
        static void connect(object sender, EventArgs e)
        {
            try
            {
                Client = new ServiceClient(new ClientCallback(), _ConnectionFactory.BuildBinding(), new EndpointAddress("net.tcp://localhost:9000/Remote"));
                Client.Register(new RegistirationObject());
            }
            catch (Exception ex)
            {
                Icon.ShowBalloonTip(5000, "Error while connecting to the server.", $"There was an error connecting to the server: {ex.Message}", ToolTipIcon.Error);
            }
        }
        static void showConsole(object sender, EventArgs e)
        {
            if (StaticConsole.InvokeRequired)
            {
                StaticConsole.Invoke((MethodInvoker)(() => StaticConsole.ShowDialog()));
            }
            else
            {
                StaticConsole.ShowDialog();
            }
        }
    }
}