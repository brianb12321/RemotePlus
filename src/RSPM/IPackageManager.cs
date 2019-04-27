using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core;

namespace RSPM
{
    public interface IPackageManager : IConnectionObject
    {
        List<Uri> Sources { get; }
        void InstallPackage(string packageName);
        void LoadPackageSources();
    }
}