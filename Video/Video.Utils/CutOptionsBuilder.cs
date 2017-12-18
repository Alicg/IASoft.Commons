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

        public IList<FFMpegCutOptions> CutOptions { get; } = new List<FFMpegCutOptions>();
        
        public IList<string> FilesToConcat { get; } = new List<string>();
        
        public string OutputExtension { get; private set; }

        private void BuildCutOptions(IList<VideoRenderOption> videoRenderOptions, Size outputSize, IGlobalExportProgress globalExportProgress)
        {
            // mkv файлы потом без проблем склеиваются в конкате. mp4, например, склеивается с артефактами.
            const string DefaultCutTempContainer = ".mkv";

            var plainConcatIsPossible = this.CheckWhetherPlainConcatIsPossible(videoRenderOptions);

            this.OutputExtension = plainConcatIsPossible ? Path.GetExtension(videoRenderOptions.First().FilePath) : DefaultCutTempContainer;

            foreach (var videoRenderOption in videoRenderOptions)
            {
                this.ProcessRenderOptions(videoRenderOption, plainConcatIsPossible, globalExportProgress, outputSize);
            }
        }

        private bool CheckWhetherPlainConcatIsPossible(IList<VideoRenderOption> videoRenderOptions)
        {
            // если все эпизоды из одного видео, то их нарезка и склейка не требуют перекодирования.
            var plainConcatIsPossible = videoRenderOptions.Distinct(v => v.FilePath).Count() == 1;
            if (plainConcatIsPossible)
            {
                if (videoRenderOptions.Any(v => v.ImagesTimeTable.Any() || !string.IsNullOrEmpty(v.OverlayText) || v.TimeWarpSettings.Any()))
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
                                     videoRenderOption.OverlayText,
                                     videoRenderOption.ImagesTimeTable);

            if (videoRenderOption.TimeWarpSettings.Any())
            {
                foreach (var timeWarpSettings in videoRenderOption.TimeWarpSettings)
                {
                    // если time warp начинается практически сразу, то нет смысла вырезать маленький кусочек в нормальной скорости.
                    if (timeWarpSettings.StartSecond > 0.5)
                    {
                        var newTempFile = this.temporaryFilesStorage.GetIntermediateFile(this.OutputExtension);
                        var cutOptionBeforeWarp = cutOptions.CloneWithTimeWarp(newTempFile, 1, 0, timeWarpSettings.StartSecond);
                        this.CutOptions.Add(cutOptionBeforeWarp);
                        this.FilesToConcat.Add(newTempFile);
                    }

                    var cutOptionWarp = cutOptions.CloneWithTimeWarp(
                        tempFile,
                        timeWarpSettings.Coefficient,
                        timeWarpSettings.StartSecond,
                        timeWarpSettings.EndSecond - timeWarpSettings.StartSecond);
                    this.CutOptions.Add(cutOptionWarp);
                    this.FilesToConcat.Add(tempFile);

                    // если time warp заканчивается практически вместе с эпизодом, то нет смысла вырезать маленький кусочек в нормальной скорости.
                    if ((videoRenderOption.DurationSeconds - timeWarpSettings.EndSecond) > 0.5)
                    {
                        var newTempFile = this.temporaryFilesStorage.GetIntermediateFile(this.OutputExtension);
                        var cutOptionBeforeWarp = cutOptions.CloneWithTimeWarp(
                            newTempFile,
                            1,
                            timeWarpSettings.EndSecond,
                            videoRenderOption.DurationSeconds - timeWarpSettings.EndSecond);
                        this.CutOptions.Add(cutOptionBeforeWarp);
                        this.FilesToConcat.Add(newTempFile);
                    }
                }
            }
            else
            {
                this.CutOptions.Add(cutOptions);
                this.FilesToConcat.Add(tempFile);
            }
        }
    }
}