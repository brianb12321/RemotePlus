using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    [DataContract]
    public class ServerExtensionCollectionProgrammer : Programmer
    {
        public ServerExtensionCollectionProgrammer()
        {
        }
        public override void Load(string file)
        {
            throw new NotImplementedException();
        }

        public override void Save(string file)
        {
            throw new NotImplementedException();
        }
    }
}
