using System;
using System.IO;
using System.Reflection;
using System.Resources;
using Utils.Extensions;

namespace Utils
{
    public static class ResourcesUtils
    {
        public static void AddResEntry(string resourceFile, string name, object value)
        {
            using (var reader = new ResourceReader(resourceFile))
            {
                using (var writer = new ResourceWriter(resourceFile))
                {
                    var de = reader.GetEnumerator();
                    while (de.MoveNext())
                    {
                        writer.AddResource(de.Entry.Key.ToString(), de.Entry.Value);
                    }
                    writer.AddResource(name, value);
                }
            }
        }

        public static void UpdateResEntry(string resourceFile, string name, object value)
        {
            using (var reader = new ResourceReader(resourceFile))
            {
                using (var writer = new ResourceWriter(resourceFile))
                {
                    var de = reader.GetEnumerator();
                    while (de.MoveNext())
                    {
                        if (de.Entry.Key.ToString().Equals(name, StringComparison.InvariantCultureIgnoreCase))
                            writer.AddResource(name, value);
                        else
                            writer.AddResource(de.Entry.Key.ToString(), de.Entry.Value.ToString());
                    }
                    writer.AddResource(name, value);
                }
            }
        }

        public static bool RemoveResEntry(string resourceFile, string resEntryKey)
        {
            var isKeyExist = false;
            using (var reader = new ResourceReader(resourceFile))
            {
                using (var writer = new ResourceWriter(resourceFile))
                {
                    var de = reader.GetEnumerator();
                    while (de.MoveNext())
                    {
                        if (!de.Entry.Key.ToString().Equals(resEntryKey, StringComparison.InvariantCultureIgnoreCase))
                        {
                            writer.AddResource(de.Entry.Key.ToString(), de.Entry.Value.ToString());
                        }
                        else
                        {
                            isKeyExist = true;
                        }
                    }
                }
            }
            return isKeyExist;
        }

        public static string UnpackResource(Assembly resourceAssembly, string namespaceName, string resourceName)
        {
            const string AppDir = "";
            var pathToResource = Path.Combine(AppDir, resourceName);
            var resourceFromResources = resourceAssembly.GetManifestResourceStream($"{namespaceName}.{resourceName}");
            if (resourceFromResources == null)
            {
                throw new ResourceUnpackException($"{namespaceName}.{resourceName} wasn't found in resources.");
            }
            if (!File.Exists(pathToResource))
            {
                resourceFromResources.WriteToFile(pathToResource);
            }
            else
            {
                var existedFileSize = new FileInfo(pathToResource).Length;
                if (resourceFromResources.Length != existedFileSize)
                {
                    resourceFromResources.WriteToFile(pathToResource);
                }
            }

            return pathToResource;
        }
    }
}
