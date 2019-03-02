using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.VFS
{
    public class BinaryFileSystemLoader : IFileSystemLoader
    {
        public IFileSystem Load()
        {
            FileStream fs = new FileStream("globalResources.rpr", FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            var system = (IFileSystem)bf.Deserialize(fs);
            return system;
        }

        public void Save(IFileSystem system)
        {
            FileStream fs = new FileStream("globalResources.rpr", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, system);
            fs.Close();
        }
    }
}