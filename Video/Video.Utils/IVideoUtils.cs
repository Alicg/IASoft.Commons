using System;
using System.Threading.Tasks;
using FFMpegWrapper;

namespace Video.Utils
{
    public interface IVideoUtils
    {
        FFMpegVideoInfo GetVideoInfo(string videoFilePath);
        byte[] GetFrameFromVideoAsByte(string videoFile, double position);
        byte[] GetFrameFromVideoAsByte(string videoFile, double position, FFMpegImageSize imageSize);
        Task<byte[]> GetFrameFromVideoAsByteAsync(string videoFile, double position);
        Task<byte[]> GetFrameFromVideoAsByteAsync(string videoFile, double position, FFMpegImageSize imageSize);

        void RenderEpisodes(VideoRenderOption[] renderOptions, string outputFile, Action<string, double> callbackAction = null, Action<double, string> finishAction = null);

        void EnableDebugMode();
        void DisableDebugMode();
    }
}