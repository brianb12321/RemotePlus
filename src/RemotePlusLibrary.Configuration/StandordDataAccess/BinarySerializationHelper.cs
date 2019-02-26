using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core;

namespace RemotePlusLibrary.Configuration.StandordDataAccess
{
    /// <summary>
    /// Provides helper methods for serializing objects to a binary file.
    /// </summary>
    public class BinarySerializationHelper : IConfigurationDataAccess
    {
        public void SaveConfig<TConfigModel>(TConfigModel obj, string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, obj);
            fs.Flush();
            fs.Close();
        }
        public TConfigModel LoadConfig<TConfigModel>(string filePath)
        {
            return LoadConfig<TConfigModel>(new FileStream(filePath, FileMode.Open, FileAccess.Read));
        }

        public TConfigModel LoadConfig<TConfigModel>(Stream configStream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            var obj = (TConfigModel)bf.Deserialize(configStream);
            configStream.Close();
            return obj;
        }
    }
}