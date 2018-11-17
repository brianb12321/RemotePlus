using System.Runtime.Serialization;

namespace RemotePlusLibrary.RequestSystem.DefaultRequestOptions
{
    [DataContract]
    public class DefaultRequestBuilder : IRequestBuilder
    {
        public string Interface { get; private set; }
        public AcquisitionMode AcqMode { get; set; }
        public DefaultRequestBuilder(string i)
        {
            Interface = i;
        }
    }
}