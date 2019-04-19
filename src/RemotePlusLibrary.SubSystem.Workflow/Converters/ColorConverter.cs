using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RemotePlusLibrary.SubSystem.Workflow.Converters
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color oldColor = (Color)value;
            System.Windows.Media.Color newColor = System.Windows.Media.Color.FromArgb(oldColor.A, oldColor.R, oldColor.G, oldColor.B);
            return newColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Windows.Media.Color oldColor = (System.Windows.Media.Color)value;
            Color newColor = Color.FromArgb(oldColor.A, oldColor.R, oldColor.G, oldColor.B);
            return newColor;
        }
    }
}