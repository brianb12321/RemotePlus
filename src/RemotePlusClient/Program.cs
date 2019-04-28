using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusLibrary.Core.IOC;
using RemotePlusClient.Utils;
using BetterLogger;
using RemotePlusClient.CommonUI.Connection;
using RemotePlusLibrary.Core;

namespace RemotePlusClient
{
    class Program : IEnvironment
    {
        public Guid EnvironmentGuid { get; set; } = Guid.NewGuid();
        public NetworkSide ExecutingSide => NetworkSide.Client;
        public EnvironmentState State { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            IOCContainer.Provider.AddSingleton<IEnvironment>(new Program());
            GlobalServices.RunningEnvironment.Start(new string[] { }).GetAwaiter().GetResult();
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

        public Task Start(string[] args)
        {
            IServiceCollection provider = IOCContainer.Provider;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Bootstrapper.InitServices(provider);
            Bootstrapper.InitKnownTypes();
            Bootstrapper.SetupErrorHandlers(IOCContainer.GetService<ILogFactory>());
            Bootstrapper.InitDialogs(provider.GetService<IDialogManager>());
            Application.Run(new Form1(provider.GetService<IDialogManager>(),
                provider.GetService<ICommandManager<MenuItem>>(),
                provider.GetService<IConnectionManager>()));
            return Task.CompletedTask;
        }
    }
}