using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Extension.ExtensionLoader;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace RemotePlusServer
{
    internal class RemotePlusWindowsService : ServiceBase
    {
        static Stopwatch sw = new Stopwatch();
        static Guid ServerGuid = Guid.NewGuid();
        public static RemoteImpl _remote = null;
        public RemotePlusWindowsService()
        {
            this.ServiceName = "RemotePlusServerService";
            this.EventLog.Log = "Application";
            this.CanShutdown = true;
            this.CanStop = true;
            this.AutoLog = false;
        }
        protected override void OnStop()
        {
            ServerStartup.Close();
            base.OnStop();
        }
        protected override void OnShutdown()
        {
            ServerStartup.Close();
            base.OnShutdown();
        }
        public void StartforDebugging()
        {
            OnStart(new string[] { });
        }
        protected override void OnStart(string[] args)
        {
            try
            {
                Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var a = Assembly.GetExecutingAssembly().GetName();
                Console.WriteLine($"Welcome to {a.Name}, version: {a.Version.ToString()}\n\n");
                sw = new Stopwatch();
                sw.Start();
                Console.WriteLine("Starting server core to setup and initialize services.");
                LoadServerCoreExtension();
                if (CheckPrerequisites())
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    ServerStartup.RunInServerMode();
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry($"RemotePlus Server Service failed to start: {ex.Message}", EventLogEntryType.Error);
            }
        }
        private void LoadServerCoreExtension()
        {
            var core = new DefaultServerCore();
            core.AddServices(new ServiceCollection());
            ServerBuilder sb = new ServerBuilder();
            core.InitializeServer(sb);
            var serverInit = sb.Build();
            serverInit.RunTasks();
        }
        static bool CheckPrerequisites()
        {
            GlobalServices.Logger.Log("Checking prerequisites.", LogLevel.Info);
            //Check for prerequisites
            ServerPrerequisites.CheckPrivilleges();
            ServerPrerequisites.CheckNetworkInterfaces();
            ServerPrerequisites.CheckSettings();
            GlobalServices.Logger.Log("Stopping stop watch.", LogLevel.Debug);
            sw.Stop();
            // Check results
            if (GlobalServices.Logger.ErrorCount >= 1 && GlobalServices.Logger.WarningCount == 0)
            {
                GlobalServices.Logger.Log($"Unable to start server. ({GlobalServices.Logger.ErrorCount} errors) Elapsed time: {sw.Elapsed.ToString()}", LogLevel.Error);
                return false;
            }
            else if (GlobalServices.Logger.ErrorCount >= 1 && GlobalServices.Logger.WarningCount >= 1)
            {
                GlobalServices.Logger.Log($"Unable to start server. ({GlobalServices.Logger.ErrorCount} errors, {GlobalServices.Logger.WarningCount} warnings) Elapsed time: {sw.Elapsed.ToString()}", LogLevel.Error);
                return false;
            }
            else if (GlobalServices.Logger.ErrorCount == 0 && GlobalServices.Logger.WarningCount >= 1)
            {
                GlobalServices.Logger.Log($"The server can start, but with warnings. ({GlobalServices.Logger.WarningCount} warnings) Elapsed time: {sw.Elapsed.ToString()}", LogLevel.Warning);
                return true;
            }
            else
            {
                GlobalServices.Logger.Log($"Validation passed. Elapsed time: {sw.Elapsed.ToString()}", LogLevel.Info);
                return true;
            }
        }
    }
}