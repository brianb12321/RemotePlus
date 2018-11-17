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
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.RequestSystem;
using RemotePlusClient.CommonUI.Requests;

namespace RemotePlusClient
{
    public class ClientApp : IEnvironment
    {
        public static MainF MainWindow;
        public static ILogFactory Logger => IOCContainer.GetService<ILogFactory>();
        public static ClientSettings ClientSettings { get; set; } = new ClientSettings();
        public static IEventBus EventBus => IOCContainer.GetService<IEventBus>();

        public NetworkSide ExecutingSide => NetworkSide.Client;

        public EnvironmentState State { get; private set; } = EnvironmentState.Created;

        [STAThread]
        static void Main(string[] args)
        {
            IOCContainer.Provider.Bind<IEnvironment>().ToConstant(new ClientApp());
            GlobalServices.RunningEnvironment.Start(args);
        }

        public void Start(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Debugger.IsAttached)
            {
                Control.CheckForIllegalCrossThreadCalls = false;
            }
            IOCContainer.Provider.Bind<ILogFactory>().ToConstant(new BaseLogFactory());
            Logger.AddLogger(new ConsoleLogger());
            IOCContainer.Provider.Bind<RemotePlusLibrary.Configuration.IConfigurationDataAccess>().To(typeof(RemotePlusLibrary.Configuration.StandordDataAccess.ConfigurationHelper)).Named("DefaultConfigDataAccess");
            IOCContainer.Provider.Bind<IEventBus>().To(typeof(EventBus)).InSingletonScope();
            Logger.Log("Loading client settings.", LogLevel.Info);
            if (File.Exists(ClientSettings.CLIENT_SETTING_PATH))
            {
                ClientSettings = new RemotePlusLibrary.Configuration.StandordDataAccess.ConfigurationHelper().LoadConfig<ClientSettings>(ClientSettings.CLIENT_SETTING_PATH);
            }
            else
            {
                Logger.Log("No config file exists. Creating new config file.", LogLevel.Warning);
                ClientSettings.DefaultTheme = Theme.AwesomeWhite;
                ClientSettings.DefaultTheme.ThemeEnabled = false;
                new RemotePlusLibrary.Configuration.StandordDataAccess.ConfigurationHelper().SaveConfig(ClientSettings, ClientSettings.CLIENT_SETTING_PATH);
            }
            InitializeDefaultKnownTypes();
            RequestStore.Add(new RequestStringRequest());
            RequestStore.Add(new ColorRequest());
            RequestStore.Add(new MessageBoxRequest());
            RequestStore.Add(new SelectLocalFileRequest());
            RequestStore.Add(new SelectFileRequest());
            RequestStore.Add(new SendFilePackageRequest());
            MainWindow = new MainF();
            MainWindow.FormClosing += (sender, e) =>
            {
                State = EnvironmentState.Closing;
                try
                {
                    MainF.Disconnect();
                }
                catch
                {
                }
            };
            State = EnvironmentState.Running;
            Application.Run(MainWindow);
        }
        static void InitializeDefaultKnownTypes()
        {
            GlobalServerBuilderExtensions.InitializeKnownTypes();
        }

        public void Close()
        {
            MainWindow.Close();
            Application.Exit();
        }
    }
}