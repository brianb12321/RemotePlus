﻿using System;
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
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            var obj = (TConfigModel)bf.Deserialize(fs);
            fs.Flush();
            fs.Close();
            return obj;
        }
    }
}