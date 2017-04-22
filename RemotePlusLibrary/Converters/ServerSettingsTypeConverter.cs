using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Converters
{
    public class ServerSettingsTypeConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if((string)value == "init")
            {
                ServerSettings ss = new ServerSettings();
                return ss;
            }
            return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
    }
}
