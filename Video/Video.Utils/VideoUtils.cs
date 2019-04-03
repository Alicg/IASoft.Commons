using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FFMpegWrapper;
using NYoutubeDL;
using NYoutubeDL.Models;

namespace Video.Utils
{
    public class VideoUtils : IVideoUtils
    {
        public FFMpegVideoInfo GetVideoInfo(string videoFilePath)
        {
            var mhandler = new FFMpeg(new TemporaryFilesStorage());
            return mhandler.GetVideoInfo(videoFilePath);
        }

        public byte[] GetFrameFromVideoAsByte(string videoFile, double position)
        {
            return this.GetFrameFromVideoAsByte(videoFile, position, FFMpegImageSize.Qqvga);
        }

        public byte[] GetFrameFromVideoAsByte(string videoFile, double position, FFMpegImageSize imageSize)
        {
            using (var tempFileStorage = new TemporaryFilesStorage())
            {
                var mhandler = new FFMpeg(tempFileStorage);
                return mhandler.GetBitmapFromVideoAsByte(videoFile, position, imageSize);
            }
        }

        public async Task<byte[]> GetFrameFromVideoAsByteAsync(string videoFile, double position)
        {
            return await this.GetFrameFromVideoAsByteAsync(videoFile, position, FFMpegImageSize.Qqvga);
        }

        public async Task<byte[]> GetFrameFromVideoAsByteAsync(string videoFile, double position, FFMpegImageSize imageSize)
        {
            return await Task.Factory.StartNew(
                () =>
                {
                    using (var tempFileStorage = new TemporaryFilesStorage())
                    {
                        var mhandler = new FFMpeg(tempFileStorage);
                        return mhandler.GetBitmapFromVideoAsByte(videoFile, position, imageSize);
                    }
                },
                TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public Task RenderEpisodesAsync(
            VideoRenderOption[] renderOptions,
            string outputFile,
            Size outputSize,
            ProcessPriorityClass processPriorityClass,
            CancellationTokenSource cancellationTokenSource = null,
            Action<string, double, double, double> callbackAction = null,
            Action<double, Exception> finishAction = null)
        {
            var renderer = new FFMpegVideoRenderer(cancellationTokenSource);
            foreach (var renderOption in renderOptions)
            {
                renderer.AddVideoEpisodes(renderOption);
            }
            return renderer.StartRenderAsync(outputFile, outputSize, processPriorityClass, callbackAction, finishAction);
        }

        public WebVideoInfo GetWebVideoInfo(string videoUrl)
        {
            var youtubeDl = new YoutubeDL();
            youtubeDl.VideoUrl = videoUrl;
            youtubeDl.RetrieveAllInfo = true;
            youtubeDl.PrepareDownload();
            var videoInfo = youtubeDl.Info as VideoDownloadInfo; 
            var randomVideoFormat = videoInfo?.RequestedFormats.FirstOrDefault(v => v.Vcodec != "none");
            var randomAudioFormat = videoInfo?.RequestedFormats.FirstOrDefault(v => v.Acodec != "none");
            if (randomVideoFormat == null)
            {
                throw new InvalidOperationException("No video stream was found by specified URL: " + videoUrl);
            }

            return new WebVideoInfo
            {
                OriginalUrl = videoUrl,
                VideoStreamUrl = randomVideoFormat.Url,
                AudioStreamUrl = randomAudioFormat?.Url,
                ThumbnailUrl = videoInfo.Thumbnail
            };
        }

        public void EnableDebugMode()
        {
            FFMpeg.DebugModeEnabled = true;
        }

        public void DisableDebugMode()
        {
            FFMpeg.DebugModeEnabled = false;
        }
    }
}
