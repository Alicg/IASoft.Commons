using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FFMpegWrapper;

namespace Video.Utils
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    public class EpisodesRenderer
    {
        private readonly CancellationToken cancellationToken;
        private readonly IList<VideoRenderOption> videoRenderOptions;
        private readonly string outputFile;
        private readonly Size outputSize;
        private readonly IGlobalExportProgress globalExportProgress;

        public EpisodesRenderer(
            IList<VideoRenderOption> videoRenderOptions,
            string outputFile,
            Size outputSize,
            IGlobalExportProgress globalExportProgress,
            CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            this.videoRenderOptions = videoRenderOptions;
            this.outputFile = outputFile;
            this.outputSize = outputSize;
            this.globalExportProgress = globalExportProgress;
        }

        public void ProcessRenderOptions()
        {
            using (var temporaryFilesStorage = new TemporaryFilesStorage())
            {
                var subject = new Subject<double>();

                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                // Внутри происходит регистрация через ссылку на родительский CancellationTokenSource.
                this.cancellationToken.Register(() => subject.OnNext(0));
                var ffMpeg = new FFMpeg(temporaryFilesStorage, subject.AsObservable());
                ffMpeg.LogMessage($"Started rendering of {this.outputFile}", string.Empty);

                var cutOptionsBuilder = new CutOptionsBuilder(this.videoRenderOptions, this.outputSize, this.globalExportProgress, temporaryFilesStorage);

                Parallel.ForEach(
                    cutOptionsBuilder.CutOptions,
                    cutOptions =>
                        {
                            if (this.cancellationToken.IsCancellationRequested)
                            {
                                this.cancellationToken.ThrowIfCancellationRequested();
                            }

                            // ReSharper disable once AccessToDisposedClosure
                            // выполнение замыкания всегда будет происходить до Dispose.
                            this.CutAndDrawTextAndDrawImageAndApplyTimeWarp(ffMpeg, cutOptions, temporaryFilesStorage);
                        });

                if (cutOptionsBuilder.FilesToConcat.Count == 1)
                {
                    File.Move(cutOptionsBuilder.FilesToConcat.Single(), this.outputFile);
                }
                else
                {
                    if (this.cancellationToken.IsCancellationRequested)
                    {
                        this.cancellationToken.ThrowIfCancellationRequested();
                    }

                    var tempFileForConcat = temporaryFilesStorage.GetIntermediateFile(cutOptionsBuilder.OutputExtension);
                    ffMpeg.Concat(tempFileForConcat, this.outputSize, this.globalExportProgress, cutOptionsBuilder.FilesToConcat.ToArray());
                    ffMpeg.Convert(tempFileForConcat, this.outputFile, this.globalExportProgress);
                }
            }
        }

        private void CutAndDrawTextAndDrawImageAndApplyTimeWarp(
            FFMpeg ffMpeg,
            FFMpegCutOptions cutOptions,
            TemporaryFilesStorage temporaryFilesStorage)
        {
            EnsureFileDoesNotExist(cutOptions.OutputFile);
            var extensionForResultFile = Path.GetExtension(cutOptions.OutputFile);
            var imagesExist = cutOptions.ImagesTimeTable != null && cutOptions.ImagesTimeTable.Any();
            var timeWarpExists = Math.Abs(cutOptions.TimeWarpCoefficient - 1) > double.Epsilon;
            if (string.IsNullOrEmpty(cutOptions.OverlayText) && !imagesExist && !timeWarpExists)
            {
                ffMpeg.Cut(cutOptions);
                return;
            }
            var intermediateFile1 = temporaryFilesStorage.GetIntermediateFile(extensionForResultFile);

            ffMpeg.Cut(cutOptions.CloneWithOtherOutput(intermediateFile1));

            if (!string.IsNullOrEmpty(cutOptions.OverlayText))
            {
                var intermediateFile2 = (imagesExist || timeWarpExists) ? temporaryFilesStorage.GetIntermediateFile(extensionForResultFile) : cutOptions.OutputFile;
                ffMpeg.DrawText(intermediateFile1, cutOptions.OverlayText, intermediateFile2, cutOptions.GlobalExportProgress);
                File.Delete(intermediateFile1);
                intermediateFile1 = intermediateFile2;
            }
            if (imagesExist)
            {
                var intermediateFile3 = timeWarpExists ? temporaryFilesStorage.GetIntermediateFile(extensionForResultFile) : cutOptions.OutputFile;
                ffMpeg.DrawImage(intermediateFile1, cutOptions.ImagesTimeTable, intermediateFile3, cutOptions.GlobalExportProgress);
                File.Delete(intermediateFile1);
                intermediateFile1 = intermediateFile3;
            }
            if (timeWarpExists)
            {
                ffMpeg.ApplyTimeWarp(intermediateFile1, cutOptions.TimeWarpCoefficient, cutOptions.OutputFile, this.globalExportProgress);
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
    }
}