using System.Runtime.Serialization;

namespace RSPM
{
    [DataContract]
    public class PackageDescription
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
}