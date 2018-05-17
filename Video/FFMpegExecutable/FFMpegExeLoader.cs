using System.IO;
using Utils.Extensions;

namespace FFMpegExecutable
{
    public static class FFMpegExeLoader
    {
        public static string UnpackFFMpegExe()
        {
            UnpackResource("avcodec-57.dll");
            UnpackResource("avdevice-57.dll");
            UnpackResource("avfilter-6.dll");
            UnpackResource("avformat-57.dll");
            UnpackResource("avutil-55.dll");
            UnpackResource("ffplay.exe");
            UnpackResource("ffprobe.exe");
            UnpackResource("postproc-54.dll");
            UnpackResource("swresample-2.dll");
            UnpackResource("swscale-4.dll");
            return UnpackResource("ffmpeg.exe");
        }

        private static string UnpackResource(string resourceName)
        {
            const string AppDir = "";
            var pathToResource = Path.Combine(AppDir, resourceName);
            var resourceFromResources = typeof(FFMpegExeLoader).Assembly.GetManifestResourceStream($"FFMpegExecutable.{resourceName}");
            if (resourceFromResources == null)
            {
                throw new FFMpegException($"FFMpegExecutable.{resourceName} wasn't found in resources.", string.Empty);
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