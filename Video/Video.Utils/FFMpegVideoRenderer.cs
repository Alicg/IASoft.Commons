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
        private readonly IList<VideoRenderOption> innerCollection = new List<VideoRenderOption>();
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
                this.innerCollection.Add(renderOption);
            }
        }

        public void StartRender(string outputFile, Action<string, double> callbackAction = null, Action<double, string> finishAction = null)
        {
            var renderStart = DateTime.Now;
            // TODO: подкоректировать в соответствии с эксперементальными затратами на конвертацию.
            // Сейчас это вырезать эпизоды, нарисовать по ним текст+штрихи и в конце один раз все склеить.
            var globalExportProgress = new GlobalExportProgress(this.innerCollection.Count * 2 + 1);
            this.ffMpeg.LogMessage($"Started rendering of {outputFile}", string.Empty);
            try
            {
                if (File.Exists(outputFile))
                    File.Delete(outputFile);

                if (this.cancellationTokenSource.Token.IsCancellationRequested)
                    this.cancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (this.innerCollection.Count == 1)
                {
                    this.ffMpeg.CutAndDrawTextAndDrawImage(this.innerCollection[0].FilePath,
                        this.innerCollection[0].StartSecond,
                        this.innerCollection[0].DurationSeconds,
                        outputFile,
                        this.innerCollection[0].OverlayText,
                        this.innerCollection[0].ImagesTimeTable,
                        p => NotifyGlobalProgress(callbackAction, p, globalExportProgress));
                }
                else
                {
                    this.CutAndConcatSeveralEpisodes(outputFile, globalExportProgress, callbackAction);
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

        private void CutAndConcatSeveralEpisodes(string outputFile,
            GlobalExportProgress globalExportProgress,
            Action<string, double> callbackAction = null)
        {
            var temporaryPartsToMerge = new List<string>();
            try
            {
                var outputExt = Path.GetExtension(outputFile);
                foreach (var renderOption in this.innerCollection)
                {
                    if (this.cancellationTokenSource.Token.IsCancellationRequested)
                        this.cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var tempFile = Path.Combine(Directory.GetCurrentDirectory(), $"{Guid.NewGuid()}{outputExt}");

                    this.ffMpeg.CutAndDrawTextAndDrawImage(renderOption.FilePath,
                        renderOption.StartSecond,
                        renderOption.DurationSeconds,
                        tempFile,
                        renderOption.OverlayText,
                        renderOption.ImagesTimeTable,
                        p => NotifyGlobalProgress(callbackAction, p, globalExportProgress));

                    temporaryPartsToMerge.Add(tempFile);
                    globalExportProgress.IncreaseOperationsDone();
                }

                if (this.cancellationTokenSource.Token.IsCancellationRequested)
                    this.cancellationTokenSource.Token.ThrowIfCancellationRequested();

                this.ffMpeg.Concat(outputFile, temporaryPartsToMerge.ToArray());
            }
            finally
            {
                foreach (var intermediateFile in temporaryPartsToMerge)
                {
                    File.Delete(intermediateFile);
                }
            }
        }

        private static void NotifyGlobalProgress(Action<string, double> callbackAction, double currentProgressPercent, GlobalExportProgress globalExportProgress)
        {
            var globalProgress = currentProgressPercent / globalExportProgress.TotalOperationsExpected + globalExportProgress.GlobalProgress;
            callbackAction?.Invoke(null, globalProgress);
        }

        private class GlobalExportProgress
        {
            private readonly int totalOperationsExpected;

            private int operationsDone;

            public GlobalExportProgress(int totalOperationsExpected)
            {
                this.totalOperationsExpected = totalOperationsExpected;
            }

            public void IncreaseOperationsDone()
            {
                this.operationsDone++;
            }

            public int TotalOperationsExpected => this.totalOperationsExpected;

            public double GlobalProgress => (double)this.operationsDone / this.totalOperationsExpected;
        }
    }
}