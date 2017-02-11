using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FFMpegWrapper;
using Utils.Extensions;

namespace Video.Utils
{
    public class FFMpegVideoRenderer : IVideoRenderer
    {
        /// <summary>
        /// Опции для вырезания и обработки каждого эпизода.
        /// </summary>
        private readonly IList<VideoRenderOption> videoRenderOptions = new List<VideoRenderOption>();

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly FFMpeg ffMpeg = new FFMpeg();

        public void Cancel()
        {
            this.cancellationTokenSource?.Cancel();
        }

        public void AddVideoEpisodes(params VideoRenderOption[] videoRenderOption)
        {
            foreach (var renderOption in videoRenderOption)
            {
                this.videoRenderOptions.Add(renderOption);
            }
        }

        public void StartRender(string outputFile, Action<string, double> callbackAction = null, Action<double, string> finishAction = null)
        {
            var renderStart = DateTime.Now;
            // TODO: подкоректировать в соответствии с эксперементальными затратами на конвертацию.
            // Сейчас это вырезать эпизоды, нарисовать по ним текст+штрихи и в конце один раз все склеить.
            var globalExportProgress = GlobalExportProgress.BuildFromRenderOptions(this.videoRenderOptions, callbackAction);
            this.ffMpeg.LogMessage($"Started rendering of {outputFile}", string.Empty);
            try
            {
                if (File.Exists(outputFile))
                    File.Delete(outputFile);

                if (this.cancellationTokenSource.Token.IsCancellationRequested)
                    this.cancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (this.videoRenderOptions.Count == 1)
                {
                    this.ffMpeg.CutAndDrawTextAndDrawImage(
                        this.videoRenderOptions[0].FilePath,
                        this.videoRenderOptions[0].StartSecond,
                        this.videoRenderOptions[0].DurationSeconds,
                        outputFile,
                        this.videoRenderOptions[0].OverlayText,
                        this.videoRenderOptions[0].ImagesTimeTable,
                        globalExportProgress);
                }
                else
                {
                    this.CutAndConcatSeveralEpisodes(outputFile, globalExportProgress);
                }

                finishAction?.Invoke((DateTime.Now - renderStart).TotalMilliseconds, null);
            }
            catch (Exception ex)
            {
                finishAction?.Invoke((DateTime.Now - renderStart).TotalMilliseconds, ex.GetFullMessage());
            }
        }

        public async void StartRenderAsync(string outputFile, Action<string, double> callbackAction, Action<double, string> finishAction)
        {
            await Task.Run(() => this.StartRender(outputFile, callbackAction, finishAction), this.cancellationTokenSource.Token);
        }

        private void CutAndConcatSeveralEpisodes(string outputFile, GlobalExportProgress globalExportProgress)
        {
            var temporaryPartsToMerge = new List<string>();
            try
            {
                var outputExt = Path.GetExtension(outputFile);
                foreach (var renderOption in this.videoRenderOptions)
                {
                    if (this.cancellationTokenSource.Token.IsCancellationRequested)
                        this.cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var tempFile = Path.Combine(Directory.GetCurrentDirectory(), $"{Guid.NewGuid()}{outputExt}");

                    this.ffMpeg.CutAndDrawTextAndDrawImage(
                        renderOption.FilePath,
                        renderOption.StartSecond,
                        renderOption.DurationSeconds,
                        tempFile,
                        renderOption.OverlayText,
                        renderOption.ImagesTimeTable,
                        globalExportProgress);

                    temporaryPartsToMerge.Add(tempFile);
                }

                if (this.cancellationTokenSource.Token.IsCancellationRequested)
                    this.cancellationTokenSource.Token.ThrowIfCancellationRequested();

                this.ffMpeg.Concat(outputFile, globalExportProgress, temporaryPartsToMerge.ToArray());
            }
            finally
            {
                foreach (var intermediateFile in temporaryPartsToMerge)
                {
                    File.Delete(intermediateFile);
                }
            }
        }
    }
}