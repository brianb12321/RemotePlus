using BetterLogger;
using BetterLogger.Loggers;
using Ninject;
using RemotePlusClient.Commands;
using RemotePlusClient.CommonUI.Connection;
using RemotePlusClient.CommonUI.Requests;
using RemotePlusClient.Dialogs;
using RemotePlusClient.Requests;
using RemotePlusClient.ViewModels;
using RemotePlusLibrary;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.RequestSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.Utils
{
    public static class Bootstrapper
    {
        public static void InitServices(IKernel provider)
        {
            DefaultDialogManager _dialogManager = new DefaultDialogManager();
            MenuItemCommandManager _commandManager = new MenuItemCommandManager();
            DefaultConnectionManager _connectManager = new DefaultConnectionManager();
            BaseLogFactory blf = new BaseLogFactory();
            blf.AddLogger(new ConsoleLogger());
            provider.Bind<ILogFactory>().ToConstant(blf);
            provider.Bind<IDialogManager>().ToConstant(_dialogManager);
            provider.Bind<ICommandManager<MenuItem>>().ToConstant(_commandManager);
            provider.Bind<IConnectionManager>().ToConstant(_connectManager);
            provider.Bind<IEventBus>().ToConstant(new EventBus(GlobalServices.Logger));
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
        public static void InitCommands(IKernel kernel)
        {
            ICommandManager<MenuItem> manager = kernel.Get<ICommandManager<MenuItem>>();
            IConnectionManager connManager = kernel.Get<IConnectionManager>();
            IDialogManager dialogManager = kernel.Get<IDialogManager>();
            IWindowManager winManager = kernel.Get<IWindowManager>();
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
            GlobalServerBuilderExtensions.InitializeKnownTypes();
        }
    }
}