using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGA.AppSettings.Writeable.NET5_Workarounds
{
    /*  These two classes address storage support for byte arrays stored as base64 strings in configuration files (like appsettings.json).
     *  This support has been included in NET6. So, this workaround is only required in NET5.
     *  See this for the original bug filing for the lack of byte array support in NET5: https://github.com/dotnet/runtime/issues/36034
     *  See this citation for the workaround for NET5: https://github.com/dotnet/runtime/issues/37384#:~:text=ericstj%20commented%20on%20Jul%207%2C%202020
     *  
     *  To enable byte array storage as base64 strings in NET5, call the static method, Add_Base64StorageSupport_forByteArray_in_NET5(),
     *  somewhere in your program.cs, before configuration builder executes.
     *  Call it only once, to prevent multiple configurationbinding instances.
     *  This is only required for NET5, as the fix was merged into the dotnet runtime in November 2020.
     */

    /// <summary>
    /// This class has been added, to provide deserialization support to NET5 for byte array data stored as base64 string in configuration properties.
    /// NOTE: This is only required for NET5, as NET6 included a type converter for byte arrays stored as base64.
    /// See this for source of the workaround for NET5: https://github.com/dotnet/runtime/issues/37384#:~:text=ericstj%20commented%20on%20Jul%207%2C%202020
    /// See this for the original bug filing for the lack of byte array support in NET5: https://github.com/dotnet/runtime/issues/36034
    /// </summary>
    static public class NET5_Base64ByteArrayStorageSupport
    {
        static private bool alreadycalled = false;

        static public void Add_Base64StorageSupport_forByteArray_in_NET5()
        {
#if NET5
            if(!alreadycalled)
                TypeDescriptor.AddAttributes(typeof(byte[]), new TypeConverterAttribute(typeof(Base64ByteArrayConverter)));
            alreadycalled = true;
#endif
        }
    }


    /// <summary>
    /// This class has been added, to provide deserialization support to NET5 for byte array data stored as base64 string in configuration properties.
    /// NOTE: This is only required for NET5, as NET6 included a type converter for byte arrays stored as base64.
    /// See this for source of the workaround for NET5: https://github.com/dotnet/runtime/issues/37384#:~:text=ericstj%20commented%20on%20Jul%207%2C%202020
    /// See this for the original bug filing for the lack of byte array support in NET5: https://github.com/dotnet/runtime/issues/36034
    /// </summary>
    public class Base64ByteArrayConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string base64)
            {
                return Convert.FromBase64String(base64);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(byte[]))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is byte[] bytes)
            {
                return Convert.ToBase64String(bytes);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
