using System.Runtime.Serialization;

namespace RemotePlusLibrary.Extension.Programmer
{
    [DataContract]
    public abstract class Programmer
    {
        public abstract void Load(string file);
        public abstract void Save(string file);
    }
}