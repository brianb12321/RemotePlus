using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusLibrary.RequestSystem
{
    /// <summary>
    /// Represents a data request to the client. A data request takes a builder, creates the request interface, the user fills out hte interface, and the request results gets sent raw through a <see cref="RawDataResponse" />>
    /// </summary>
    /// <typeparam name="TBuilder">The data builder used for sending data to the request.</typeparam>
    public interface IDataRequest<TBuilder, TUpdateBuilder> : IRequestAdapter
        where TBuilder : RequestBuilder
        where TUpdateBuilder : UpdateRequestBuilder
    {
        /// <summary>
        /// Starts the data request process.
        /// </summary>
        /// <param name="builder">The information that the data-request will se to build the request interface.</param>
        /// <param name="executingSide">The side in which the request is being executed on.</param>
        /// <returns>The data directly received from the interfaces</returns>
        RawDataResponse RequestData(TBuilder builder, NetworkSide executingSide);
        void Update(TUpdateBuilder message);
    }
}