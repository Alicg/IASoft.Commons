using Utils;

namespace FFMpegExecutable
{
    public static class FFMpegExeLoader
    {
        public static string UnpackFFMpegExe()
        {
            var thisAssembly = typeof(FFMpegExeLoader).Assembly;
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "avcodec-57.dll");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "avdevice-57.dll");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "avfilter-6.dll");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "avformat-57.dll");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "avutil-55.dll");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "ffplay.exe");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "ffprobe.exe");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "postproc-54.dll");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "swresample-2.dll");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "swscale-4.dll");
            return ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "ffmpeg.exe");
        }
    }
}