using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FFMpegExecutable;
using FFMpegWrapper;

namespace Video.Utils
{
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    /// <summary>
    /// Производить отрисовку эпизодов по следующему алгоритму:
    /// 1. Вырезает и соединяет идущие подряд эпизоды из одного источника без конверсии.
    /// 2. Соединяет группы эпизодов из разных источников с конверсией.
    /// 3. Применяет эффекты один раз на результирующее видео с полным набором эпизодов.
    /// </summary>
    public class EpisodesRendererAllFiltersInSingleCommands : IEpisodesRenderer
    {
        private readonly CancellationToken cancellationToken;
        private readonly IList<VideoRenderOption> videoRenderOptions;
        private readonly string outputFile;
        private readonly Size outputSize;
        private readonly ProcessPriorityClass rendererProcessPriorityClass;
        private readonly IGlobalExportProgress globalExportProgress;

        public EpisodesRendererAllFiltersInSingleCommands(
            IList<VideoRenderOption> videoRenderOptions,
            string outputFile,
            Size outputSize,
            ProcessPriorityClass rendererProcessPriorityClass,
            IGlobalExportProgress globalExportProgress,
            CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            this.videoRenderOptions = videoRenderOptions;
            this.outputFile = outputFile;
            this.outputSize = outputSize;
            this.rendererProcessPriorityClass = rendererProcessPriorityClass;
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
                var ffMpeg = new FFMpeg(temporaryFilesStorage, this.rendererProcessPriorityClass, subject.AsObservable());
                ffMpeg.LogMessage($"Started rendering of {this.outputFile}", string.Empty);

                var cutInfos = this.videoRenderOptions.Select(v => new FFMpegCutInfo(v.FilePath, v.StartSecond, v.StartSecond + v.DurationSeconds)).ToList();
                
                this.CutAndConcatAndRenderTextAndImageAndTimeWarps(cutInfos, ffMpeg, temporaryFilesStorage);
            }
        }

        private void CutAndConcatAndRenderTextAndImageAndTimeWarps(List<FFMpegCutInfo> cutInfos, FFMpeg ffMpeg, TemporaryFilesStorage temporaryFilesStorage)
        {
            var textTimeTableForConcatenatedEpisodeGroups = new List<TextTimeRecord>();
            var imagesTimeTableForConcatenatedEpisodeGroups = new List<DrawImageTimeRecord>();
            var timeWarpForConcatenatedEpisodeGroups = new List<TimeWarpRecord>();
            var currentPosition = 0.0;
            foreach (var videoRenderOption in this.videoRenderOptions)
            {
                if (videoRenderOption.OverlayTextTimeTable != null && videoRenderOption.OverlayTextTimeTable.Any())
                {
                    textTimeTableForConcatenatedEpisodeGroups.AddRange(
                        videoRenderOption.OverlayTextTimeTable.Select(v => new TextTimeRecord(v.Lines,
                            v.StartSecond + currentPosition,
                            v.EndSecond + currentPosition)));
                }
                if (videoRenderOption.ImagesTimeTable != null && videoRenderOption.ImagesTimeTable.Any())
                {
                    imagesTimeTableForConcatenatedEpisodeGroups.AddRange(
                        videoRenderOption.ImagesTimeTable.Select(v => new DrawImageTimeRecord(v.ImageData,
                            v.LeftOffset,
                            v.TopOffset,
                            v.ImageStartSecond + currentPosition,
                            v.ImageEndSecond + currentPosition)));
                }
                if (videoRenderOption.TimeWarpSettings != null && videoRenderOption.TimeWarpSettings.Any())
                {
                    timeWarpForConcatenatedEpisodeGroups.AddRange(
                        videoRenderOption.TimeWarpSettings.Select(v =>
                            new TimeWarpRecord(v.StartSecond + currentPosition, v.EndSecond + currentPosition, v.Coefficient)));
                }
                currentPosition += videoRenderOption.DurationSeconds;
            }

            var pathForConcatenatedEpisodes = timeWarpForConcatenatedEpisodeGroups.Any()
                    ? temporaryFilesStorage.GetIntermediateFile(Path.GetExtension(this.outputFile))
                    : this.outputFile;

            ffMpeg.CutAndConcatAndDrawImagesAndText(
                cutInfos,
                imagesTimeTableForConcatenatedEpisodeGroups,
                textTimeTableForConcatenatedEpisodeGroups,
                this.outputSize,
                pathForConcatenatedEpisodes,
                this.globalExportProgress);

            if (timeWarpForConcatenatedEpisodeGroups.Any())
            {
                ffMpeg.ApplyTimeWarp(pathForConcatenatedEpisodes, timeWarpForConcatenatedEpisodeGroups, this.outputFile, this.globalExportProgress);
            }
        }
    }
}