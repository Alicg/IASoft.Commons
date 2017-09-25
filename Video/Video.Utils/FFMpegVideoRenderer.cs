using System;
using System.Collections.Generic;
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

        private readonly FFMpeg ffMpeg;

        public FFMpegVideoRenderer(CancellationTokenSource cancellationTokenSource = null)
        {
            this.cancellationToken = cancellationTokenSource?.Token ?? CancellationToken.None;
            var subject = new Subject<double>();

            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            // Внутри происходит регистрация через ссылку на родительский CancellationTokenSource.
            this.cancellationToken.Register(() => subject.OnNext(0));
            this.ffMpeg = new FFMpeg(PresetParameters.Medium, subject.AsObservable());
        }

        public void AddVideoEpisodes(params VideoRenderOption[] videoRenderOption)
        {
            foreach (var renderOption in videoRenderOption)
            {
                this.videoRenderOptions.Add(renderOption);
            }
        }

        public void StartRender(string outputFile, Action<string, double> callbackAction = null, Action<double, Exception> finishAction = null)
        {
            var renderStart = DateTime.Now;
            // TODO: подкоректировать в соответствии с эксперементальными затратами на конвертацию.
            // Сейчас это вырезать эпизоды, нарисовать по ним текст+штрихи и в конце один раз все склеить.
            var globalExportProgress = GlobalExportProgress.BuildFromRenderOptions(this.videoRenderOptions, callbackAction);
            this.ffMpeg.LogMessage($"Started rendering of {outputFile}", string.Empty);
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

                if (this.videoRenderOptions.Count == 1)
                {
                    this.CutAndDrawTextAndDrawImage(
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
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }
                finishAction?.Invoke((DateTime.Now - renderStart).TotalMilliseconds, ex);
            }
        }

        public Task StartRenderAsync(string outputFile, Action<string, double> callbackAction, Action<double, Exception> finishAction)
        {
            return Task.Run(() => this.StartRender(outputFile, callbackAction, finishAction), this.cancellationToken);
        }

        private void CutAndConcatSeveralEpisodes(string outputFile, GlobalExportProgress globalExportProgress)
        {
            var temporaryPartsToMerge = new List<string>();
            try
            {
                var outputExt = Path.GetExtension(outputFile);
                foreach (var renderOption in this.videoRenderOptions)
                {
                    if (this.cancellationToken.IsCancellationRequested)
                        this.cancellationToken.ThrowIfCancellationRequested();

                    var tempFile = GetIntermediateFile(outputExt);

                    this.CutAndDrawTextAndDrawImage(
                        renderOption.FilePath,
                        renderOption.StartSecond,
                        renderOption.DurationSeconds,
                        tempFile,
                        renderOption.OverlayText,
                        renderOption.ImagesTimeTable,
                        globalExportProgress);

                    temporaryPartsToMerge.Add(tempFile);
                }

                if (this.cancellationToken.IsCancellationRequested)
                    this.cancellationToken.ThrowIfCancellationRequested();

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

        public void CutAndDrawTextAndDrawImage(
            string inputFile,
            double start,
            double end,
            string outputFile,
            string overlayText,
            List<DrawImageTimeRecord> imagesTimeTable,
            IGlobalExportProgress globalExportProgress)
        {
            EnsureFileDoesNotExist(outputFile);
            const string ExtensionForResultFile = ".avi";
            var imagesExist = imagesTimeTable != null && imagesTimeTable.Any();
            if (string.IsNullOrEmpty(overlayText) && !imagesExist)
            {
                this.ffMpeg.Cut(start, end, inputFile, outputFile, globalExportProgress);
                return;
            }
            var intermediateFile1 = GetIntermediateFile(ExtensionForResultFile);

            this.ffMpeg.Cut(start, end, inputFile, intermediateFile1, globalExportProgress);

            if (!string.IsNullOrEmpty(overlayText))
            {
                var intermediateFile2 = imagesExist ? GetIntermediateFile(ExtensionForResultFile) : outputFile;
                this.ffMpeg.DrawText(intermediateFile1, overlayText, intermediateFile2, globalExportProgress);
                File.Delete(intermediateFile1);
                intermediateFile1 = intermediateFile2;
            }
            if (imagesExist)
            {
                this.ffMpeg.DrawImage(intermediateFile1, imagesTimeTable, outputFile, globalExportProgress);
                File.Delete(intermediateFile1);
            }
        }

        private static void EnsureFileDoesNotExist(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private static string GetIntermediateFile(string ext)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), $"{Guid.NewGuid()}{ext}");
        }
    }
}