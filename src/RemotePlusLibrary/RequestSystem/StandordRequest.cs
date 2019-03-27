using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core;

namespace RemotePlusLibrary.RequestSystem
{
    /// <summary>
    /// The default implementation of <see cref="IDataRequest{TBuilder}"/>
    /// </summary>
    public abstract class StandordRequest<TBuilder, TUpdateBuilder> : IDataRequest<TBuilder, TUpdateBuilder>
        where TBuilder : RequestBuilder
        where TUpdateBuilder : UpdateRequestBuilder
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
        protected virtual void UpdateBuilderInvalid(UpdateRequestBuilder message)
        {

        }

        public virtual void Update(TUpdateBuilder message)
        {
            
        }

        public void StartUpdate(UpdateRequestBuilder message)
        {
            if (message is TUpdateBuilder)
            {
                Update((TUpdateBuilder)message);
            }
            else
            {
                UpdateBuilderInvalid(message);
            }
        }

        public virtual void Dispose()
        {
        }
    }
}