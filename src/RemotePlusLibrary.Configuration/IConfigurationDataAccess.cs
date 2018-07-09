using System.Collections.Generic;

namespace RemotePlusLibrary.Configuration
{
    public interface IConfigurationDataAccess
    {
        void SaveConfig<TConfigModel>(TConfigModel configType, string file);
        TConfigModel LoadConfig<TConfigModel>(string file);
    }
}