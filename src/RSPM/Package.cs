using Ionic.Zip;

namespace RSPM
{
    public class Package
    {
        public string Path { get; private set; }
        public ZipFile Zip { get; private set; }
        public PackageDescription Description { get; private set; }
        public Package(string fileName, PackageDescription desc, ZipFile zip)
        {
            Path = fileName;
            Description = desc;
            Zip = zip;
        }
    }
}