using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;
using Ionic.Zip;

namespace RSPM
{
    public class DefaultPackageReader : IPackageReader
    {
        public Package BuildPackage(string path)
        {
            var zip = ZipFile.Read(path);
            var desc = readPackageManifest(zip);
            return new Package(path, desc, zip);
        }
        PackageDescription readPackageManifest(ZipFile zip)
        {
            if (zip.ContainsEntry("package.manifest"))
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(PackageDescription));
                XmlReader reader = XmlReader.Create(zip.First(e => e.FileName == "package.manifest").OpenReader());
                var ss = (PackageDescription)ser.ReadObject(reader);
                reader.Close();
                return ss;
            }
            else
            {
                return null;
            }
        }
    }
}