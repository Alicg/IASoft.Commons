using Utils;

namespace FFMpegExecutable
{
    public static class FFMpegExeLoader
    {
        public static string UnpackFFMpegExe()
        {
            var thisAssembly = typeof(FFMpegExeLoader).Assembly;
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "avcodec-58.dll");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "avdevice-58.dll");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "avfilter-7.dll");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "avformat-58.dll");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "avutil-56.dll");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "ffplay.exe");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "ffprobe.exe");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "postproc-55.dll");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "swresample-3.dll");
            ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "swscale-5.dll");
            return ResourcesUtils.UnpackResource(thisAssembly, "FFMpegExecutable", "ffmpeg.exe");
        }
    }
}