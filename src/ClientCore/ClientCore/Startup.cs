using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Windows.Forms;
using BetterLogger.Loggers;
using RemotePlusClient.CommonUI;
using RemotePlusClientCmd;
using RemotePlusClientCmd.ClientExtensionSystem;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Core.NodeStartup;
using RemotePlusLibrary;
using RemotePlusLibrary.Configuration;
using RemotePlusLibrary.Configuration.StandordDataAccess;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.Faults;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing;

namespace ClientCmdCore
{
    public class Startup : IClientCoreStartup
    {
        public void AddServices(IServiceCollection services)
        {
            services.UseLogger(factory =>
            {
                factory.AddLogger(new ConsoleLogger());
            });
            services.UseExtensionSystem<RemotePlusLibrary.Extension.DefaultExtensionLoader>()
                .UseCommandline<CommandEnvironment, ClientCmdExtensionSubsystem, IClientCmdModule>(builder =>
                {
                    builder.UseLexer<CommandLexer>();
                    builder.UseParser<CommandParser>();
                    builder.UseExecutor<ClientCommandExecutor>();
                })
                .UseNotifyIcon()
                .UseConfigurationDataAccess<ConfigurationHelper>()
                .UseScriptingEngine<IronPythonScriptingEngine>()
                .UseResourceManager<RemotePlusResourceManager, FileResourceLoader>();
        }

        public void InitializeNode(IClientBuilder builder)
        {
            builder.SetupApplicationPrerequisites()
                .AddExceptionHandler((sender, e) =>
                {
                    if (e.ExceptionObject is FaultException<ServerFault> exception)
                    {
                        CatchException(exception);
                    }
                    else
                    {
                        CatchException((Exception)e.ExceptionObject);
                    }
                    if (e.IsTerminating)
                    {
                        Environment.Exit(-1);
                        IOCContainer.GetService<NotifyIcon>().Dispose();
                    }
                })
                .SetupNotifyIcon()
                .LoadExtensionLibraries()
                .LoadExtensionByType(typeof(ClientCmdCommands))
                .LoadDefaultExtensionSubsystems<IClientBuilder, ICommandSubsystem<IClientCmdModule>, IClientCmdModule>()
                .ConfigureRequests()
                .AddTask(() =>
                {
                    RequestStore.Add(new RemotePlusClientCmd.Requests.ConsoleMenuRequest());
                    RequestStore.Add(new RemotePlusClientCmd.Requests.SelectableConsoleMenu());
                    RequestStore.Add(new RemotePlusClientCmd.Requests.RCmdMessageBox());
                    RequestStore.Add(new RemotePlusClientCmd.Requests.RCmdTextBox());
                    RequestStore.Add(new RemotePlusClientCmd.Requests.RCmdMultiLineTextbox());
                    RequestStore.Add(new RemotePlusClientCmd.Requests.ConsoleProgressRequest());
                    RequestStore.Add(new RemotePlusClientCmd.Requests.ConsoleReadLineRequest());
                })
                .InitializeKnownTypes();
        }
        void CatchException(Exception ex)
        {
            using (ErrorDialog d = new ErrorDialog(ex))
            {
                d.ShowDialog();
            }
        }
        public void PostInitializeNode(IClientBuilder builder)
        {
        }
    }
}
