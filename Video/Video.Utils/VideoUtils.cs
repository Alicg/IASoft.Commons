using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using FFMpegWrapper;

namespace Video.Utils
{
    public class VideoUtils : IVideoUtils
    {
        public FFMpegVideoInfo GetVideoInfo(string videoFilePath)
        {
            var mhandler = new FFMpeg();
            return mhandler.GetVideoInfo(videoFilePath);
        }

        public byte[] GetFrameFromVideoAsByte(string videoFile, double position)
        {
            return this.GetFrameFromVideoAsByte(videoFile, position, FFMpegImageSize.Qqvga);
        }

        public byte[] GetFrameFromVideoAsByte(string videoFile, double position, FFMpegImageSize imageSize)
        {
            var mhandler = new FFMpeg();
            return mhandler.GetBitmapFromVideoAsByte(videoFile, position, imageSize);
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
                    var mhandler = new FFMpeg();
                    return mhandler.GetBitmapFromVideoAsByte(videoFile, position, imageSize);
                },
                TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public Task RenderEpisodesAsync(
            VideoRenderOption[] renderOptions,
            string outputFile,
            Size outputSize,
            CancellationTokenSource cancellationTokenSource = null,
            Action<string, double, double, double> callbackAction = null,
            Action<double, Exception> finishAction = null)
        {
            var renderer = new FFMpegVideoRenderer(cancellationTokenSource);
            foreach (var renderOption in renderOptions)
            {
                renderer.AddVideoEpisodes(renderOption);
            }
            return renderer.StartRenderAsync(outputFile, outputSize, callbackAction, finishAction);
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
