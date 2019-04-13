using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSPM
{
    public class PackageManagerBuilder
    {
        public PackageManagerBuilder SetPackageDownloader<TPackageDownloader>() where TPackageDownloader : IPackageDownloader
        {
            IOCContainer.Provider.AddTransient<IPackageDownloader, TPackageDownloader>();
            return this;
        }
        public PackageManagerBuilder SetPackageSourceReader<TPackageSourceReaderImpl>() where TPackageSourceReaderImpl : ISourceReader
        {
            IOCContainer.Provider.AddTransient<IPackageDownloader, TPackageSourceReaderImpl>();
            return this;
        }
    }
}
