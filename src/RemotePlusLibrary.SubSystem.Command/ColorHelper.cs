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
        public static Color FromRgbArray(string[] array)
        {
            switch (array.Length)
            {
                case 0:
                    throw new Exception("An empty array is not valid.");
                case 1:
                    return Color.FromArgb(int.Parse(array[0]));
                case 3:
                    return Color.FromArgb(int.Parse(array[0]),
                        int.Parse(array[1]),
                        int.Parse(array[2]));
                case 4:
                    return Color.FromArgb(int.Parse(array[0]),
                        int.Parse(array[1]),
                        int.Parse(array[2]),
                        int.Parse(array[3]));
                default:
                    throw new ArgumentException("Too many elements in arrays.");
            }
        }
    }
}