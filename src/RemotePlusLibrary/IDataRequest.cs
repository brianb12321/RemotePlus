using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusLibrary
{
    /// <summary>
    /// Represents a data request to the client. A data request takes a builder, creates the request interface, the user fills out hte interface, and the request results gets sent raw through a <see cref="RawDataRequest" />>
    /// </summary>
    public interface IDataRequest
    {
        /// <summary>
        /// Starts the data request process.
        /// </summary>
        /// <param name="builder">The information that the data-request will se to build the request interface.</param>
        /// <returns>The data directly received from the interfaces</returns>
        RawDataRequest RequestData(RequestBuilder builder);
        /// <summary>
        /// Determines whether the end user can hit the properties button in the show data-request screen.
        /// </summary>
        bool ShowProperties { get; }
        /// <summary>
        /// The display name of the data-request.
        /// </summary>
        string FriendlyName { get; }
        /// <summary>
        /// The description of the data-request.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Updates the properties.
        /// </summary>
        void UpdateProperties();
    }
}