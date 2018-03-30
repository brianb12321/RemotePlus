using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace RemotePlusLibrary.Scripting.ScriptPackageEngine
{
    public class ScriptPackage
    {
        public ZipFile PackageContents { get; private set; }
        public ScriptPackageManifest PackageManifest { get; private set; }
        public string PackageFileName { get; set; }
        private ScriptPackage(ZipFile contents, ScriptPackageManifest manifest, string packageFileName)
        {
            PackageContents = contents;
            PackageManifest = manifest;
            PackageFileName = packageFileName;
        }
        public static ScriptPackage Open(string packageName)
        {
            if(!File.Exists(packageName))
            {
                throw new FileNotFoundException($"{packageName} does not exist.");
            }
            ZipFile package = ZipFile.Read(packageName);
            var manifest = ReadPackageManifest(package[$"{Path.GetFileNameWithoutExtension(packageName)}/{Path.GetFileNameWithoutExtension(packageName)}.manifest"]);
            return new ScriptPackage(package, manifest, Path.GetFileNameWithoutExtension(packageName));
        }

        private static ScriptPackageManifest ReadPackageManifest(ZipEntry zipArchiveEntry)
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(ScriptPackageManifest));
            XmlReader reader = XmlReader.Create(zipArchiveEntry.OpenReader());
            var ss = (ScriptPackageManifest)ser.ReadObject(reader);
            reader.Close();
            return ss;
        }
    }
}
