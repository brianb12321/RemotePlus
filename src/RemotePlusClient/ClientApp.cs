using Logging;
using RemotePlusClient.CommonUI;
using RemotePlusLibrary;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.Gui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient
{
    public class ClientApp
    {
        public static MainF MainWindow;
        public static CMDLogging Logger { get; private set; }
        public static ClientSettings ClientSettings { get; } = new ClientSettings();
        [STAThread]
        static void Main(string[] args)
        {
            Logger = new CMDLogging()
            {
                DefaultFrom = "Client",
                OverrideLogItemObjectColorValue = true
            };
            Logger.AddOutput("Loading client settings.", OutputLevel.Info);
            if(File.Exists(ClientSettings.CLIENT_SETTING_PATH))
            {
                ClientSettings.Load();
            }
            else
            {
                Logger.AddOutput("No config file exists. Creating new config file.", OutputLevel.Warning);
                ClientSettings.DefaultTheme = Theme.AwesomeWhite;
                ClientSettings.DefaultTheme.ThemeEnabled = false;
                ClientSettings.Save();
            }
            InitializeDefaultKnownTypes();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RequestStore.Init();
            MainWindow = new MainF();
            Application.Run(MainWindow);
        }
        static void InitializeDefaultKnownTypes()
        {
            Logger.AddOutput("Initializing default known types.", OutputLevel.Info);
            DefaultKnownTypeManager.LoadDefaultTypes();
            DefaultKnownTypeManager.AddType(typeof(UserAccount));
        }
    }
}
