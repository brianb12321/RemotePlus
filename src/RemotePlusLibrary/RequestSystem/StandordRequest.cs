using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.ExtensionLoader;

namespace RemotePlusLibrary.RequestSystem
{
    /// <summary>
    /// The default implementation of <see cref="IDataRequest{TBuilder}"/>
    /// </summary>
    public abstract class StandordRequest<TBuilder> : IDataRequest<TBuilder> where TBuilder : RequestBuilder
    {
        public abstract string URI { get; }
        public abstract bool ShowProperties { get; }
        public abstract string FriendlyName { get; }
        public abstract string Description { get; }
        public abstract NetworkSide SupportedSides { get; }

        public abstract RawDataResponse RequestData(TBuilder builder, NetworkSide executingSide);

        public RawDataResponse StartRequestData(RequestBuilder builder, NetworkSide executingSide)
        {
            if(builder is TBuilder)
            {
                return RequestData((TBuilder)builder, executingSide);
            }
            else
            {
                return BuilderInvalid(builder);
            }
        }

        protected virtual RawDataResponse BuilderInvalid(RequestBuilder builder)
        {
            throw new RequestException($"The request builder is invalid for request: {URI}");
        }

        public abstract void Update(string message);
    }
}
