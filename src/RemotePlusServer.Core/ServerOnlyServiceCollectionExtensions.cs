using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BetterLogger;
using RemotePlusLibrary.Configuration;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.NodeStartup;
using RemotePlusLibrary.Security.AccountSystem;

namespace RemotePlusServer.Core
{
    public static class ServerOnlyServiceCollectionExtensions
    {
        public static IServiceCollection LoadServerSettings(this IServiceCollection services)
        {
            ServerSettings settings = new ServerSettings();
            IConfigurationDataAccess dataAccess = services.GetServiceNamed<IConfigurationDataAccess>("DefaultDataAccess");
            IAccountManager accountManager = services.GetService<IAccountManager>();
            if (!File.Exists("Configurations\\Server\\GlobalServerSettings.config"))
            {
                RemotePlusLibrary.Configuration.ServerSettings.ServerSettings internalSettings = loadConfigFromAssembly();
                if (internalSettings != null)
                {
                    GlobalServices.Logger.Log("Server settings loaded from assembly.", LogLevel.Info);
                    settings = internalSettings;
                }
                else
                {
                    GlobalServices.Logger.Log("The server settings file does not exist. Creating server settings file.", LogLevel.Warning);
                    dataAccess.SaveConfig(settings, RemotePlusLibrary.Configuration.ServerSettings.ServerSettings.SERVER_SETTINGS_FILE_PATH);
                }
            }
            else
            {
                GlobalServices.Logger.Log("Loading server settings file.", LogLevel.Info);
                try
                {
                    settings = dataAccess.LoadConfig<RemotePlusLibrary.Configuration.ServerSettings.ServerSettings>(RemotePlusLibrary.Configuration.ServerSettings.ServerSettings.SERVER_SETTINGS_FILE_PATH);
                }
                catch (Exception ex)
                {
#if DEBUG
                    GlobalServices.Logger.Log("Unable to load server settings. " + ex.ToString(), LogLevel.Error);
#else
                    GlobalServices.Logger.Log("Unable to load server settings. " + ex.Message, LogLevel.Error);
#endif
                }
            }
            if (!Directory.Exists("Users"))
            {
                const string DEFAULT_USERNAME = "admin";
                const string DEFAULT_PASSWORD = "password";
                if (settings.UseDefaultUserIfNoneExists)
                {
                    GlobalServices.Logger.Log("The user folder does not exist. Using default user.", LogLevel.Info);
                    accountManager.CreateAccount(new UserCredentials(DEFAULT_USERNAME, DEFAULT_PASSWORD), false);
                }
                else
                {
                    GlobalServices.Logger.Log("The Users folder does not exist. Creating folder.", LogLevel.Warning);
                    Directory.CreateDirectory("Users");
                    accountManager.CreateAccount(new UserCredentials(DEFAULT_USERNAME, DEFAULT_PASSWORD));
                }
            }
            else
            {
                ServerManager.AccountManager.RefreshAccountList();
            }

            return services.AddSingleton(settings);
        }
        /// <summary>
        /// Preloads <see cref="ServerSettings"/> with the perferred settings. NOTE: not calling <see cref="SkipServerSettingsLoading(IServerTaskBuilder)"/> will override your preferred settings.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection PreloadSettings(this IServiceCollection services, Action<ServerSettings> options)
        {
            ServerSettings s = new ServerSettings();
            options?.Invoke(s);
            return services.AddSingleton(s);
        }
        private static ServerSettings loadConfigFromAssembly()
        {
            try
            {
                Assembly loadedAssembly = Assembly.GetEntryAssembly();
                GlobalServices.Logger.Log($"Scanning for embedded config file in assembly: {loadedAssembly.GetName()}", LogLevel.Debug);
                var stream = loadedAssembly.GetManifestResourceStream("RemotePlusServer.InternalConfig.config");
                return IOCContainer.GetService<IConfigurationDataAccess>("DefaultDataAccess").LoadConfig<RemotePlusLibrary.Configuration.ServerSettings.ServerSettings>(stream);
            }
            catch (Exception ex)
            {
                GlobalServices.Logger.Log($"Unable to open internal config file: {ex.Message}", LogLevel.Debug);
                return null;
            }
        }
    }
}