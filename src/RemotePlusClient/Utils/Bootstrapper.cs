using BetterLogger;
using BetterLogger.Loggers;
using RemotePlusClient.Commands;
using RemotePlusClient.CommonUI.Connection;
using RemotePlusClient.CommonUI.Requests;
using RemotePlusClient.Dialogs;
using RemotePlusClient.Requests;
using RemotePlusLibrary;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.RequestSystem;
using System.Windows.Forms;
using RemotePlusLibrary.Core.IOC;

namespace RemotePlusClient.Utils
{
    public static class Bootstrapper
    {
        public static void InitServices(IServiceCollection provider)
        {
            DefaultDialogManager dialogManager = new DefaultDialogManager();
            MenuItemCommandManager commandManager = new MenuItemCommandManager();
            DefaultConnectionManager connectManager = new DefaultConnectionManager();
            BaseLogFactory blf = new BaseLogFactory();
            blf.AddLogger(new ConsoleLogger());
            provider.AddSingleton<ILogFactory>(blf);
            provider.AddSingleton<IDialogManager>(dialogManager);
            provider.AddSingleton<ICommandManager<MenuItem>>(commandManager);
            provider.AddSingleton<IConnectionManager>(connectManager);
            provider.AddSingleton<IEventBus>(new EventBus(GlobalServices.Logger));
        }

        public static void InitRequests(IConnectionManager manager)
        {
            RequestStore.Init();
            RequestStore.Add(new RequestStringRequest());
            RequestStore.Add(new ColorRequest());
            RequestStore.Add(new MessageBoxRequest());
            RequestStore.Add(new SelectLocalFileRequest());
            RequestStore.Add(new SelectFileRequest());
            RequestStore.Add(new SendFilePackageRequest());
            RequestStore.Add(new ProgressRequest());
        }

        public static void InitDialogs(IDialogManager manager)
        {
            manager.AddDialog(new ConnectDialog());
            manager.AddDialog(new AuthenticationDialog());
        }
        public static void InitCommands(IServiceCollection kernel)
        {
            ICommandManager<MenuItem> manager = kernel.GetService<ICommandManager<MenuItem>>();
            IConnectionManager connManager = kernel.GetService<IConnectionManager>();
            IDialogManager dialogManager = kernel.GetService<IDialogManager>();
            IWindowManager winManager = kernel.GetService<IWindowManager>();
            manager.AddCommand("Exit", new ExitCommand());
            manager.AddCommand("Connect", new ConnectCommand(connManager, dialogManager));
            manager.AddCommand("Open Console", new OpenConsoleCommand(winManager, connManager));
        }

        internal static void SetupErrorHandlers(ILogFactory logger)
        {
            Application.ThreadException += (sender, e) =>
            {
                logger.Log($"An unhandled error occurred: {e.Exception.Message}", LogLevel.Error);
            };
        }

        public static void InitKnownTypes()
        {
            GlobalNodeBuilderExtensions.InitializeKnownTypes();
        }
    }
}