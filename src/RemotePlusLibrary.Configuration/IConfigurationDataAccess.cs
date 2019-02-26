using System.Collections.Generic;
using System.IO;

namespace RemotePlusLibrary.Configuration
{
    public interface IConfigurationDataAccess
    {
        void SaveConfig<TConfigModel>(TConfigModel configType, string file);
        TConfigModel LoadConfig<TConfigModel>(string file);
        TConfigModel LoadConfig<TConfigModel>(Stream configStream);
    }
}