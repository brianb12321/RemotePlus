using System;

namespace RSPM
{
    /// <summary>
    /// Downloads a package from a package source.
    /// </summary>
    public interface IPackageDownloader
    {
        bool DownlaodPackage(string packageName, Uri[] sources);
    }
}