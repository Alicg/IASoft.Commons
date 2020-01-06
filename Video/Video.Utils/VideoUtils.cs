using System;
using System.ComponentModel;
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
            using (var mhandler = new FFMpeg(new TemporaryFilesStorage()))
            {
                return mhandler.GetVideoInfo(videoFilePath);
            }
        }

        public byte[] GetFrameFromVideoAsByte(string videoFile, double positionMs)
        {
            return this.GetFrameFromVideoAsByte(videoFile, positionMs, FFMpegImageSize.Qqvga);
        }

        public byte[] GetFrameFromVideoAsByte(string videoFile, double positionMs, FFMpegImageSize imageSize)
        {
            using (var tempFileStorage = new TemporaryFilesStorage())
            {
                using (var mhandler = new FFMpeg(tempFileStorage))
                {
                    return mhandler.GetBitmapFromVideoAsByte(videoFile, positionMs, imageSize);
                }
            }
        }

        public async Task<byte[]> GetFrameFromVideoAsByteAsync(string videoFile, double positionMs)
        {
            return await this.GetFrameFromVideoAsByteAsync(videoFile, positionMs, FFMpegImageSize.Qqvga);
        }

        public async Task<byte[]> GetFrameFromVideoAsByteAsync(string videoFile, double positionMs, FFMpegImageSize imageSize)
        {
            return await await Task.Factory.StartNew(
                async () =>
                {
                    using (var tempFileStorage = new TemporaryFilesStorage())
                    {
                        using (var mhandler = new FFMpeg(tempFileStorage))
                        {
                            return mhandler.GetBitmapFromVideoAsByte(videoFile, positionMs, imageSize);
                        }
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

        [Description("Использовать только для тестов. Приводит в дедлоку на UI.")]
        public WebVideoInfo GetWebVideoInfo(string videoUrl, bool singleBest = false)
        {
            var getWebVideoInfoTask = this.GetWebVideoInfoAsync(videoUrl, singleBest);
            getWebVideoInfoTask.Wait();
            return getWebVideoInfoTask.Result;
        }

        public async Task<WebVideoInfo> GetWebVideoInfoAsync(string videoUrl, bool singleBest = false, bool videoBest = false)
        {
            var youtubeDl = new YoutubeDL();
            youtubeDl.VideoUrl = videoUrl;
            youtubeDl.RetrieveAllInfo = true;
            var videoInfo = await youtubeDl.GetDownloadInfoAsync() as VideoDownloadInfo;// PrepareDownloadAsync();
            FormatDownloadInfo videoFormat;
            FormatDownloadInfo audioFormat = null;
            if (singleBest)
            {
                videoFormat = videoInfo?.Formats.Where(v => v.Vcodec != "none" && v.Acodec != "none").OrderBy(v => v.Height).ThenBy(v => v.Width)
                    .LastOrDefault();
            }
            else if (videoBest)
            {
                videoFormat = videoInfo?.Formats.Where(v => v.Vcodec != "none").OrderBy(v => v.Height).ThenBy(v => v.Width)
                    .LastOrDefault();
            }
            else
            {
                videoFormat = videoInfo?.RequestedFormats.FirstOrDefault(v => v.Vcodec != "none");
                audioFormat = videoInfo?.RequestedFormats.FirstOrDefault(v => v.Acodec != "none");
            }
            if (videoFormat == null)
            {
                throw new InvalidOperationException("No video stream was found by specified URL: " + videoUrl);
            }

            return new WebVideoInfo
            {
                OriginalUrl = videoUrl,
                VideoStreamUrl = videoFormat.Url,
                AudioStreamUrl = audioFormat?.Url,
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

        public static bool IsInternetVideo(string videoSource)
        {
            return videoSource.Contains("https://") || videoSource.Contains("http://");
        }
    }
}
