using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FFMpegWrapper;

namespace Video.Utils
{
    public class EpisodesRenderer
    {
        private readonly FFMpeg ffMpeg;
        private readonly CancellationToken cancellationToken;
        private readonly IList<VideoRenderOption> videoRenderOptions;
        private readonly string outputFile;
        private readonly Size outputSize;
        private readonly IGlobalExportProgress globalExportProgress;

        public EpisodesRenderer(
            FFMpeg ffMpeg,
            IList<VideoRenderOption> videoRenderOptions,
            string outputFile,
            Size outputSize,
            IGlobalExportProgress globalExportProgress,
            CancellationToken cancellationToken)
        {
            this.ffMpeg = ffMpeg;
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
                var cutOptionsBuilder = new CutOptionsBuilder(this.videoRenderOptions, this.outputSize, this.globalExportProgress, temporaryFilesStorage);
            
                Parallel.ForEach(
                    this.videoRenderOptions,
                    renderOption =>
                    {
                        if (this.cancellationToken.IsCancellationRequested)
                        {
                            this.cancellationToken.ThrowIfCancellationRequested();
                        }

                        var cutOptions = cutOptionsBuilder.CutOptions[renderOption];
                        
                        // ReSharper disable once AccessToDisposedClosure
                        // выполнение замыкания всегда будет происходить до Dispose.
                        this.CutAndDrawTextAndDrawImage(cutOptions, renderOption.OverlayText, renderOption.ImagesTimeTable, temporaryFilesStorage);
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
                    this.ffMpeg.Concat(tempFileForConcat, this.outputSize, this.globalExportProgress, cutOptionsBuilder.FilesToConcat.ToArray());
                    this.ffMpeg.Convert(tempFileForConcat, this.outputFile, this.globalExportProgress);
                }
            }
        }

        private void CutAndDrawTextAndDrawImage(
            FFMpegCutOptions cutOptions,
            string overlayText,
            List<DrawImageTimeRecord> imagesTimeTable,
            TemporaryFilesStorage temporaryFilesStorage)
        {
            EnsureFileDoesNotExist(cutOptions.OutputFile);
            var extensionForResultFile = Path.GetExtension(cutOptions.OutputFile);
            var imagesExist = imagesTimeTable != null && imagesTimeTable.Any();
            if (string.IsNullOrEmpty(overlayText) && !imagesExist)
            {
                this.ffMpeg.Cut(cutOptions);
                return;
            }
            var intermediateFile1 = temporaryFilesStorage.GetIntermediateFile(extensionForResultFile);

            this.ffMpeg.Cut(cutOptions.CloneWithOtherOutput(intermediateFile1));

            if (!string.IsNullOrEmpty(overlayText))
            {
                var intermediateFile2 = imagesExist ? temporaryFilesStorage.GetIntermediateFile(extensionForResultFile) : cutOptions.OutputFile;
                this.ffMpeg.DrawText(intermediateFile1, overlayText, intermediateFile2, cutOptions.GlobalExportProgress);
                File.Delete(intermediateFile1);
                intermediateFile1 = intermediateFile2;
            }
            if (imagesExist)
            {
                this.ffMpeg.DrawImage(intermediateFile1, imagesTimeTable, cutOptions.OutputFile, cutOptions.GlobalExportProgress);
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