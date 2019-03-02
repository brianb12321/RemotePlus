using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.VFS
{
    public struct UPath
    {
        public static char DefaultSeperator => '/';
        public string Path { get; set; }
        public string GetLastElement()
        {
            string[] separatedPath = Path.Split(DefaultSeperator);
            for(int i = 0; i < separatedPath.Length; i++)
            {
                if (i == separatedPath.Length - 1) return separatedPath[i];
            }
            throw new Exception("Unable to find last path element.");
        }
        public UPath(string path)
        {
            if (!path.StartsWith(DefaultSeperator.ToString())) throw new Exception("Path must start with " + DefaultSeperator);
            else
            {
                Path = path;
                swapCharacters();
            }
        }

        private void swapCharacters()
        {
            Path.Replace('\\', '/');
        }
        public string[] GetPathElements()
        {
            return Path.Substring(1).Split(DefaultSeperator);
        }
    }
}