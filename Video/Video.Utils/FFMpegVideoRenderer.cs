using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using FFMpegWrapper;

namespace Video.Utils
{
    public class FFMpegVideoRenderer : IVideoRenderer
    {
        private readonly CancellationToken cancellationToken;

        /// <summary>
        /// Опции для вырезания и обработки каждого эпизода.
        /// </summary>
        private readonly IList<VideoRenderOption> videoRenderOptions = new List<VideoRenderOption>();

        public FFMpegVideoRenderer(CancellationTokenSource cancellationTokenSource = null)
        {
            this.cancellationToken = cancellationTokenSource?.Token ?? CancellationToken.None;
        }

        public void AddVideoEpisodes(params VideoRenderOption[] videoRenderOption)
        {
            foreach (var renderOption in videoRenderOption)
            {
                this.videoRenderOptions.Add(renderOption);
            }
        }

        public void StartRender(string outputFile, Action<string, double, double, double> callbackAction = null, Action<double, Exception> finishAction = null)
        {
            this.StartRender(outputFile, Size.Empty, callbackAction, finishAction);
        }

        public void StartRender(string outputFile, Size outputSize, Action<string, double, double, double> callbackAction = null, Action<double, Exception> finishAction = null)
        {
            var renderStart = DateTime.Now;
            
            // TODO: подкоректировать в соответствии с эксперементальными затратами на конвертацию.
            // Сейчас это вырезать эпизоды, нарисовать по ним текст+штрихи и в конце один раз все склеить.
            var globalExportProgress = GlobalExportProgress.BuildFromRenderOptions(this.videoRenderOptions, callbackAction);
            try
            {
                try
                {
                    if (File.Exists(outputFile))
                    {
                        File.Delete(outputFile);
                    }

                    if (this.cancellationToken.IsCancellationRequested)
                    {
                        this.cancellationToken.ThrowIfCancellationRequested();
                    }

                    var episodesRenderer = new EpisodesRenderer(this.videoRenderOptions, outputFile, outputSize, globalExportProgress, this.cancellationToken);
                    episodesRenderer.ProcessRenderOptions();

                    finishAction?.Invoke((DateTime.Now - renderStart).TotalMilliseconds, null);
                }
                catch (AggregateException aggregateException)
                {
                    var isAggregateAndAllInnerAreCancelledExceptions = aggregateException.InnerExceptions.All(e => e is FFMpegCancelledException);
                    if (isAggregateAndAllInnerAreCancelledExceptions)
                    {
                        var ffmpegOutputs = aggregateException.InnerExceptions.Cast<FFMpegCancelledException>().Select(v => v.AllFFMpegOutput);
                        var aggregatedOutputs = ffmpegOutputs.Aggregate((t, c) => t + "\r\n" + c);
                        throw new FFMpegCancelledException("Process was cancelled", $"Aggregated exceptions:\r\n{aggregatedOutputs}");
                    }
                    throw;
                }
                catch (OperationCanceledException)
                {
                    throw new FFMpegCancelledException("Process was cancelled", "No FFMPeg log available. Cancelled between FFMPeg calls.");
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }
                finishAction?.Invoke((DateTime.Now - renderStart).TotalMilliseconds, ex);
            }
        }

        public Task StartRenderAsync(string outputFile, Size outputSize, Action<string, double, double, double> callbackAction, Action<double, Exception> finishAction)
        {
            return Task.Run(() => this.StartRender(outputFile, outputSize, callbackAction, finishAction), this.cancellationToken);
        }
    }
}