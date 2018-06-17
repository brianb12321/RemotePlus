using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Configuration
{
    /// <summary>
    /// Provides helper methods for serializing objects to a binary file.
    /// </summary>
    public class BinarySerializationHelper<T>
    {
        public static void SaveObject(string filePath, T obj)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, obj);
            fs.Flush();
            fs.Close();
        }
        public static T OpenObject(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            var obj = (T)bf.Deserialize(fs);
            fs.Flush();
            fs.Close();
            return obj;
        }
    }
}
