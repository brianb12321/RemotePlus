using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.AccountSystem
{
    [DataContract]
    public class SecurityPolicyFolder
    {
        [DataMember]
        public SecurityPolicyFolder ParentFolder { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Path { get; set; }
        public string FullPath => getFullPath();
        [DataMember]
        public List<SecurityPolicy> Policies { get; private set; } = new List<SecurityPolicy>();
        [DataMember]
        public List<SecurityPolicyFolder> Folders { get; private set; } = new List<SecurityPolicyFolder>();
        public SecurityPolicyFolder FindSubFolder(string folderName)
        {
            if (folderName == Name)
                return this;

            SecurityPolicyFolder result = null;
            foreach (var c in Folders)
            {
                result = c.FindSubFolder(folderName);
                if (result != null)
                    break;
            }
            return result;
        }
        private string getFullPath()
        {
            var fullPath = Path;
            var parent = ParentFolder;
            while (parent != null)
            {
                fullPath = String.Format("{0}{1}{2}", parent.FullPath, System.IO.Path.DirectorySeparatorChar, fullPath);
            }
            return fullPath;
        }
    }
}