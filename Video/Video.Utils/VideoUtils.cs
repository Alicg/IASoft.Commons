using System.Threading.Tasks;
using Utils.Extensions;

namespace Video.Utils
{
    using System.IO;

    using FFMpegWrapper;

    public static class VideoUtils
    {
        private const string AppDir = "";

        public static string FfmpegPath
        {
            get
            {
                var pathToFFMpeg = Path.Combine(AppDir, "ffmpeg.exe");
                if (!File.Exists(pathToFFMpeg))
                {
                    typeof(MediaHandler).Assembly.GetManifestResourceStream("FFMpegWrapper.ffmpeg.exe").WriteToFile(pathToFFMpeg);
                }
                return pathToFFMpeg;
            }
        }

        public static string FontsPath
        {
            get
            {
                var pathToFFMpeg = Path.Combine(AppDir, "arialbd.ttf");
                if (!File.Exists(pathToFFMpeg))
                {
                    typeof(MediaHandler).Assembly.GetManifestResourceStream("FFMpegWrapper.arialbd.ttf").WriteToFile(pathToFFMpeg);
                }
                return pathToFFMpeg;
            }
        }

        public static void Init()
        {}

        public static FFMpegVideoInfo GetVideoInfo(string videoFilePath)
        {
            var mhandler = new MediaHandler(FfmpegPath, FontsPath);
            return mhandler.GetVideoInfo(videoFilePath);
        }

        public static byte[] GetFrameFromVideoAsByte(string videoFile, double position)
        {
            return GetFrameFromVideoAsByte(videoFile, position, FFMpegImageSize.qqvga);
        }

        public static byte[] GetFrameFromVideoAsByte(string videoFile, double position, FFMpegImageSize imageSize)
        {
            var mhandler = new MediaHandler(FfmpegPath, FontsPath);
            return mhandler.GetBitmapFromVideoAsByte(videoFile, position, imageSize);
        }

        public static async Task<byte[]> GetFrameFromVideoAsByteAsync(string videoFile, double position)
        {
            return await GetFrameFromVideoAsByteAsync(videoFile, position, FFMpegImageSize.qqvga);
        }

        public static async Task<byte[]> GetFrameFromVideoAsByteAsync(string videoFile, double position, FFMpegImageSize imageSize)
        {
            return await Task.Factory.StartNew(() =>
            {
                var mhandler = new MediaHandler(FfmpegPath, FontsPath);
                return mhandler.GetBitmapFromVideoAsByte(videoFile, position, imageSize);
            });
        }

        public static void EnableDebugMode()
        {
            MediaHandler.DebugModeEnabled = true;
        }

        public static void DisableDebugMode()
        {
            MediaHandler.DebugModeEnabled = false;
        }
    }
}
