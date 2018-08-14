using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSPM
{
    public class DefaultPackageReader : IPackageReader
    {
        public Package BuildPackage(string path)
        {
            return new Package(path);
        }
    }
}
