using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command
{
    public static class ColorHelper
    {
        public static Color FromRGBArray(string[] array)
        {
            if (array.Length == 0) throw new Exception("An empty array is not valid.")
            else if (array.Length == 1) return Color.FromArgb(int.Parse(array[0]));
            else if (array.Length == 3) return Color.FromArgb(int.Parse(array[0]),
                int.Parse(array[1]),
                int.Parse(array[2]));
            else if (array.Length == 4) return Color.FromArgb(int.Parse(array[0]),
                int.Parse(array[1]),
                int.Parse(array[2)),
                int.Parse(array[3]);
            else throw new ArgumentException("Too many elements in arrays.");
        }
    }
}