using System;
using RemotePlusLibrary.Core;

namespace RSPM
{
    /// <summary>
    /// Downloads a package from a package source.
    /// </summary>
    public interface IPackageDownloader : IConnectionObject
    {
        bool DownlaodPackage(string packageName, Uri[] sources);
    }
}