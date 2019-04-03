using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using FFMpegWrapper;
using Utils.Extensions;

namespace Video.Utils
{
    [Obsolete("Не используется более.")]
    public class CutOptionsBuilder
    {
        // mkv файлы потом без проблем склеиваются в конкате. mp4, например, склеивается с артефактами.
        public const string DefaultCutTempContainer = ".mkv";

        private readonly TemporaryFilesStorage temporaryFilesStorage;

        public CutOptionsBuilder(IList<VideoRenderOption> videoRenderOptions, Size outputSize, IGlobalExportProgress globalExportProgress, TemporaryFilesStorage temporaryFilesStorage, bool ignoreOverlays)
        {
            this.temporaryFilesStorage = temporaryFilesStorage;
            this.BuildCutOptions(videoRenderOptions, outputSize, globalExportProgress, ignoreOverlays);
        }

        public IList<FFMpegCutOptions> CutOptions { get; } = new List<FFMpegCutOptions>();
        
        public IList<string> FilesToConcat { get; } = new List<string>();
        
        public string OutputExtension { get; private set; }

        private void BuildCutOptions(IList<VideoRenderOption> videoRenderOptions, Size outputSize, IGlobalExportProgress globalExportProgress, bool ignoreOverlays)
        {
            var plainConcatIsPossible = this.CheckWhetherPlainConcatIsPossible(videoRenderOptions, ignoreOverlays);

            this.OutputExtension = plainConcatIsPossible ? Path.GetExtension(videoRenderOptions.First().FilePath) : DefaultCutTempContainer;

            foreach (var videoRenderOption in videoRenderOptions)
            {
                this.ProcessRenderOptions(videoRenderOption, plainConcatIsPossible, globalExportProgress, outputSize);
            }
        }

        private bool CheckWhetherPlainConcatIsPossible(IList<VideoRenderOption> videoRenderOptions, bool ignoreOverlays)
        {
            if (videoRenderOptions.Any(v => v.FilePath.Contains("https://") || v.FilePath.Contains("http://")))
            {
                return false;
            }
            // если все эпизоды из одного видео, то их нарезка и склейка не требуют перекодирования.
            var plainConcatIsPossible = videoRenderOptions.Distinct(v => v.FilePath).Count() == 1;
            if (plainConcatIsPossible)
            {
                if (!ignoreOverlays && (videoRenderOptions.Any(v => v.ImagesTimeTable.Any() || v.OverlayTextTimeTable.Any() || v.TimeWarpSettings.Any())))
                {
                    plainConcatIsPossible = false;
                }

                // divx не склеиваются без кодирования.
                if (videoRenderOptions.Any(v => Path.GetExtension(v.FilePath) == ".divx"))
                {
                    plainConcatIsPossible = false;
                }
            }
            return plainConcatIsPossible;
        }

        private void ProcessRenderOptions(VideoRenderOption videoRenderOption, bool plainConcatIsPossible, IGlobalExportProgress globalExportProgress, Size outputSize)
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
                                     outputSize,
                                     videoRenderOption.OverlayTextTimeTable,
                                     videoRenderOption.ImagesTimeTable,
                                     videoRenderOption.TimeWarpSettings);

            this.CutOptions.Add(cutOptions);
            this.FilesToConcat.Add(tempFile);
        }
    }
}