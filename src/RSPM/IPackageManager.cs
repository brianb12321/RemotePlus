using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSPM
{
    public interface IPackageManager
    {
        List<Uri> Sources { get; }
        void InstallPackage(string packageName);
        void LoadPackageSources();
    }
}