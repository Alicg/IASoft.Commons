using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FFMpegWrapper;
using NYoutubeDL;
using NYoutubeDL.Helpers;
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
            var getWebVideoInfoTask = this.GetWebVideoInfoAsync(videoUrl);
            getWebVideoInfoTask.Wait();
            return getWebVideoInfoTask.Result;
        }

        public async Task<WebVideoInfo> GetWebVideoInfoAsync(string videoUrl)
        {
            var youtubeDl = new YoutubeDL();
            youtubeDl.VideoUrl = videoUrl;
            youtubeDl.RetrieveAllInfo = true;
            var videoInfo = await youtubeDl.GetDownloadInfoAsync() as VideoDownloadInfo;// PrepareDownloadAsync();
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
                ThumbnailUrl = videoInfo.Thumbnail,
                VideoTitle = videoInfo.Title,
                VideoDescription = videoInfo.Description,
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

        public async Task DownloadYoutubeVideo(string videoUrl, string localPath, Action<string, string, string> progressCallback)
        {
            var youtubeDl = new YoutubeDL();
            youtubeDl.VideoUrl = videoUrl;
            youtubeDl.Options.FilesystemOptions.Output = localPath;
            youtubeDl.Options.PostProcessingOptions.ExtractAudio = false;
            youtubeDl.Options.DownloadOptions.FragmentRetries = -1;
            youtubeDl.Options.DownloadOptions.Retries = -1;
            youtubeDl.Options.VideoFormatOptions.Format = Enums.VideoFormat.best;
            youtubeDl.Options.PostProcessingOptions.AudioFormat = Enums.AudioFormat.best;
            youtubeDl.Options.PostProcessingOptions.AudioQuality = "0";

            youtubeDl.StandardOutputEvent += (sender, s) =>
            {
                var pattern = @"(?<progress>[^ ]{1,10}%).*?at.*?(?<rate>\d[^ ]{1,10}MiB\/s).*?(?<ETA>ETA [^ ]*)";
                var match = Regex.Match(s, pattern);
                if (match.Success)
                {
                    progressCallback(match.Groups["progress"].Value, match.Groups["rate"].Value, match.Groups["ETA"].Value);
                }
            };

            
            await youtubeDl.DownloadAsync();
        }
    }
}
