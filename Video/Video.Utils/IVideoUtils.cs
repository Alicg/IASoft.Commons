using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using FFMpegWrapper;

namespace Video.Utils
{
    public interface IVideoUtils
    {
        FFMpegVideoInfo GetVideoInfo(string videoFilePath);
        
        WebVideoInfo GetWebVideoInfo(string videoUrl, bool singleBest = false);
        Task<WebVideoInfo> GetWebVideoInfoAsync(string videoUrl, bool singleBest = false, bool videoBest = false);
        byte[] GetFrameFromVideoAsByte(string videoFile, double position);
        byte[] GetFrameFromVideoAsByte(string videoFile, double position, FFMpegImageSize imageSize);
        Task<byte[]> GetFrameFromVideoAsByteAsync(string videoFile, double positionMs);
        Task<byte[]> GetFrameFromVideoAsByteAsync(string videoFile, double position, FFMpegImageSize imageSize);

        Task RenderEpisodesAsync(
            VideoRenderOption[] renderOptions,
            string outputFile,
            Size outputSize,
            ProcessPriorityClass processPriorityClass,
            CancellationTokenSource cancellationTokenSource = null,
            Action<string, double, double, double> callbackAction = null,
            Action<double, Exception> finishAction = null);

        void EnableDebugMode();
        void DisableDebugMode();

        Task DownloadYoutubeVideo(string videoUrl, string localPath, Action<string, string, string> progressCallback);
    }
}