using System;

namespace RemotePlusLibrary.Core
{
    /// <summary>
    /// Specifies which side of the network the component is on. This enum uses flags.
    /// </summary>
    [Flags]
    public enum NetworkSide
    {
        Server = 1,
        Proxy = 2,
        Client = 4
    }
}