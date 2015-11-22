using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Utils.Extensions
{
    public static class AssemblyExtension
    {
        [Localizable(false)]
        public static Stream GetResourceStream(this Assembly assembly, string relativeResourceName)
        {
            string name = assembly.GetName().Name + "." + relativeResourceName;
            return assembly.GetManifestResourceStream(name);
        }

        [Localizable(false)]
        public static TextReader GetResourceReader(this Assembly assembly, string relativeResourceName)
        {
            string name = assembly.GetName().Name + "." + relativeResourceName;
            Stream stream = assembly.GetManifestResourceStream(name);
            return stream == null ? null : new StreamReader(stream);
        }

        [Localizable(false)]
        public static string GetResourceAllText(this Assembly assembly, string relativeResourceName)
        {
            string name = assembly.GetName().Name + "." + relativeResourceName;
            using (Stream stream = assembly.GetManifestResourceStream(name))
            {
                if (stream == null) return null;
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}