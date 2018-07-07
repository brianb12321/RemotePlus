using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BetterLogger;
using BetterLogger.Loggers;
using NewRemotePlusClient.IOC;
using RemotePlusLibrary.IOC;

namespace NewRemotePlusClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            BaseLogFactory blf = new BaseLogFactory();
            blf.AddLogger(new ConsoleLogger());
            IOCContainer.Provider.Bind<ILogFactory>().ToConstant(blf);
            GlobalServices.Logger.Log("Starting up application.", BetterLogger.LogLevel.Info);
            ViewModels.MainWindowViewModel viewModel = new ViewModels.MainWindowViewModel();
            IOCContainer.Provider.Bind<ViewModels.MainWindowViewModel>().ToConstant(viewModel);
            IOCContainer.Provider.Bind<IUIManager>().ToConstant(new UIManager());
        }
    }
}
