using Ionic.Zip;

namespace RSPM
{
    public class Package
    {
        public string Path { get; private set; }
        public ZipFile Zip { get; private set; }
        public Package(string fileName)
        {
            Path = fileName;
            Zip = ZipFile.Read(fileName);
        }
    }
}