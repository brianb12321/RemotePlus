using RemotePlusLibrary.Core;
using System;

namespace RemotePlusLibrary.RequestSystem
{
    public interface IRequestAdapter : IDisposable
    {
        string URI { get; }
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
        /// Determines whether a request can be executed on a specified side.
        /// </summary>
        NetworkSide SupportedSides { get; }
        void StartUpdate(UpdateRequestBuilder message);
        RawDataResponse StartRequestData(RequestBuilder builder, NetworkSide executingSide);
    }
}