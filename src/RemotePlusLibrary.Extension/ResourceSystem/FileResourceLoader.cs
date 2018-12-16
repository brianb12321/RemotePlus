using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ResourceSystem
{
    public class FileResourceLoader : IResourceLoader
    {
        public ResourceStore Load()
        {
            FileStream fs = new FileStream("globalResources.rpr", FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            var store = (ResourceStore)bf.Deserialize(fs);
            return store;
        }

        public void Save(ResourceStore store)
        {
            FileStream fs = new FileStream("globalResources.rpr", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, store);
            fs.Close();
        }
    }
}