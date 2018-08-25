using RemotePlusClient.CommonUI;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.Gui;
using System;
using System.IO;
using System.Windows.Forms;
using RemotePlusLibrary.Security.AccountSystem;
using System.Diagnostics;
using RemotePlusClient.Settings;
using BetterLogger;
using BetterLogger.Loggers;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary;
using RemotePlusLibrary.Extension.EventSystem;

namespace RemotePlusClient
{
    public class ClientApp
    {
        public static MainF MainWindow;
        public static ILogFactory Logger { get; private set; }
        public static ClientSettings ClientSettings { get; set; } = new ClientSettings();
        public static IEventBus EventBus => IOCContainer.GetService<IEventBus>();
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Debugger.IsAttached)
            {
                Control.CheckForIllegalCrossThreadCalls = false;
            }
            Logger = new BaseLogFactory();
            Logger.AddLogger(new ConsoleLogger());
            IOCContainer.Provider.Bind<RemotePlusLibrary.Configuration.IConfigurationDataAccess>().To(typeof(RemotePlusLibrary.Configuration.StandordDataAccess.ConfigurationHelper)).Named("DefaultConfigDataAccess");
            IOCContainer.Provider.Bind<IEventBus>().To(typeof(EventBus)).InSingletonScope();
            Logger.Log("Loading client settings.", LogLevel.Info);
            if(File.Exists(ClientSettings.CLIENT_SETTING_PATH))
            {
                ClientSettings = GlobalServices.DataAccess.LoadConfig<ClientSettings>(ClientSettings.CLIENT_SETTING_PATH);
            }
            else
            {
                Logger.Log("No config file exists. Creating new config file.", LogLevel.Warning);
                ClientSettings.DefaultTheme = Theme.AwesomeWhite;
                ClientSettings.DefaultTheme.ThemeEnabled = false;
                GlobalServices.DataAccess.SaveConfig(ClientSettings, ClientSettings.CLIENT_SETTING_PATH);
            }
            InitializeDefaultKnownTypes();
            RequestStore.Init();
            RequestStore.Add("global_selectFile", new SelectFileRequest());
            RequestStore.Add("global_sendFilePackage", new SendFilePackageRequest());
            MainWindow = new MainF();
            Application.Run(MainWindow);
        }

        static void InitializeDefaultKnownTypes()
        {
            Logger.Log("Initializing default known types.", LogLevel.Info);
            DefaultKnownTypeManager.LoadDefaultTypes();
            DefaultKnownTypeManager.AddType(typeof(UserAccount));
        }
    }
}
