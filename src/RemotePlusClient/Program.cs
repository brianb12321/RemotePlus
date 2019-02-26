using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusLibrary.Core.IOC;
using Ninject;
using RemotePlusClient.Utils;
using BetterLogger;
using RemotePlusClient.CommonUI.Connection;
using RemotePlusLibrary.Core;

namespace RemotePlusClient
{
    class Program : IEnvironment
    {
        public NetworkSide ExecutingSide => NetworkSide.Client;
        public EnvironmentState State { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            IOCContainer.Provider.Bind<IEnvironment>().ToConstant(new Program());
            GlobalServices.RunningEnvironment.Start(new string[] { });
        }

        public void Close()
        {
            try
            {
                IOCContainer.GetService<IConnectionManager>().Close();
            }
            catch { }
            finally { Application.Exit(); }
        }

        public void Start(string[] args)
        {
            IKernel provider = IOCContainer.Provider;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Bootstrapper.InitServices(provider);
            Bootstrapper.InitKnownTypes();
            Bootstrapper.SetupErrorHandlers(IOCContainer.GetService<ILogFactory>());
            Bootstrapper.InitDialogs(provider.Get<IDialogManager>());
            Application.Run(new Form1(provider.Get<IDialogManager>(),
                provider.Get<ICommandManager<MenuItem>>(),
                provider.Get<IConnectionManager>()));
        }
    }
}