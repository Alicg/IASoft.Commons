using System.IO;
using Utils.Extensions;

namespace FFMpegExecutable
{
    public static class FFMpegExeLoader
    {
        public static string UnpackFFMpegExe()
        {
            const string AppDir = "";
            var pathToFfMpegExe = Path.Combine(AppDir, "ffmpeg.exe");
            var ffMpegFromResources = typeof(FFMpegExeLoader).Assembly.GetManifestResourceStream("FFMpegExecutable.ffmpeg.exe");
            if (ffMpegFromResources == null)
            {
                throw new FFMpegException("FFMpegExecutable.ffmpeg.exe wasn't found in resources.", string.Empty);
            }
            if (!File.Exists(pathToFfMpegExe))
            {
                ffMpegFromResources.WriteToFile(pathToFfMpegExe);
            }
            else
            {
                var existedFileSize = new FileInfo(pathToFfMpegExe).Length;
                if (ffMpegFromResources.Length != existedFileSize)
                {
                    ffMpegFromResources.WriteToFile(pathToFfMpegExe);
                }
            }

            return pathToFfMpegExe;
        }
    }
}