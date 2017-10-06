using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using FFMpegWrapper;
using Utils.Extensions;

namespace Video.Utils
{
    public class CutOptionsBuilder
    {
        private readonly TemporaryFilesStorage temporaryFilesStorage;

        public CutOptionsBuilder(IList<VideoRenderOption> videoRenderOptions, Size outputSize, IGlobalExportProgress globalExportProgress, TemporaryFilesStorage temporaryFilesStorage)
        {
            this.temporaryFilesStorage = temporaryFilesStorage;
            this.BuildCutOptions(videoRenderOptions, outputSize, globalExportProgress);
        }

        public Dictionary<VideoRenderOption, FFMpegCutOptions> CutOptions { get; } = new Dictionary<VideoRenderOption, FFMpegCutOptions>();
        
        public IList<string> FilesToConcat = new List<string>();
        
        public string OutputExtension { get; private set; }

        private void BuildCutOptions(IList<VideoRenderOption> videoRenderOptions, Size outputSize, IGlobalExportProgress globalExportProgress)
        {
            // mkv файлы потом без проблем склеиваются в конкате. mp4, например, склеивается с артефактами.
            const string DefaultCutTempContainer = ".mkv";

            // если все эпизоды из одного видео, то их нарезка и склейка не требуют перекодирования.
            var plainConcatIsPossible = videoRenderOptions.Distinct(v => v.FilePath).Count() == 1;
            if (plainConcatIsPossible)
            {
                if (videoRenderOptions.Any(v => v.ImagesTimeTable.Any() || !string.IsNullOrEmpty(v.OverlayText)))
                {
                    plainConcatIsPossible = false;
                }

                // divx не склеиваются без кодирования.
                if (videoRenderOptions.Any(v => Path.GetExtension(v.FilePath) == ".divx"))
                {
                    plainConcatIsPossible = false;
                }
            }
            this.OutputExtension = plainConcatIsPossible ? Path.GetExtension(videoRenderOptions.First().FilePath) : DefaultCutTempContainer;

            foreach (var videoRenderOption in videoRenderOptions)
            {
                var tempFile = this.temporaryFilesStorage.GetIntermediateFile(this.OutputExtension);

                var cutOptions = plainConcatIsPossible
                    ? FFMpegCutOptions.BuildSimpleCatOptions(
                        videoRenderOption.FilePath,
                        tempFile,
                        videoRenderOption.StartSecond,
                        videoRenderOption.DurationSeconds,
                        globalExportProgress)
                    : FFMpegCutOptions.BuildCatOptionsWithConvertations(
                        videoRenderOption.FilePath,
                        tempFile,
                        videoRenderOption.StartSecond,
                        videoRenderOption.DurationSeconds,
                        globalExportProgress,
                        outputSize);
                this.CutOptions.Add(videoRenderOption, cutOptions);
                this.FilesToConcat.Add(tempFile);
            }
        }
    }
}