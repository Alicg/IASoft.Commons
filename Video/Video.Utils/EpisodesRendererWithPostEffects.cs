using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    public class EpisodesRendererWithPostEffects : IEpisodesRenderer
    {
        private readonly CancellationToken cancellationToken;
        private readonly IList<VideoRenderOption> videoRenderOptions;
        private readonly string outputFile;
        private readonly Size outputSize;
        private readonly ProcessPriorityClass rendererProcessPriorityClass;
        private readonly IGlobalExportProgress globalExportProgress;

        public EpisodesRendererWithPostEffects(
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

                var simpleEpisodeGroupsToConcat = this.RenderGroupsOfSameFormatEpisodes(ffMpeg, temporaryFilesStorage);
                //var pathForConcatenatedEpisodeGroups = this.ConcatAllEpisodeGroups(simpleEpisodeGroupsToConcat, ffMpeg, temporaryFilesStorage);
                this.ConcatAndRenderTextAndImageAndTimeWarps(simpleEpisodeGroupsToConcat, ffMpeg, temporaryFilesStorage);
            }
        }

        private List<string> RenderGroupsOfSameFormatEpisodes(FFMpeg ffMpeg, TemporaryFilesStorage temporaryFilesStorage)
        {
            string previousSource = null;
            var simpleEpisodeGroupsToConcat = new List<string>();
            var renderOptionsForSimpleCut = new List<VideoRenderOption>();
            foreach (var videoRenderOption in this.videoRenderOptions)
            {
                if (!string.IsNullOrEmpty(previousSource) && videoRenderOption.FilePath != previousSource)
                {
                    var pathForGroupOfSimpleEpisodes =
                        this.RenderGroup(ffMpeg, renderOptionsForSimpleCut, temporaryFilesStorage);
                    simpleEpisodeGroupsToConcat.Add(pathForGroupOfSimpleEpisodes);
                    renderOptionsForSimpleCut.Clear();
                }
                renderOptionsForSimpleCut.Add(videoRenderOption);
                previousSource = videoRenderOption.FilePath;
            }
            if (renderOptionsForSimpleCut.Any())
            {
                var pathForGroupOfSimpleEpisodes =
                    this.RenderGroup(ffMpeg, renderOptionsForSimpleCut, temporaryFilesStorage);
                simpleEpisodeGroupsToConcat.Add(pathForGroupOfSimpleEpisodes);
            }

            return simpleEpisodeGroupsToConcat;
        }

        private string ConcatAllEpisodeGroups(List<string> simpleEpisodeGroupsToConcat, FFMpeg ffMpeg, TemporaryFilesStorage temporaryFilesStorage)
        {
            var pathForConcatenatedEpisodeGroups =
                temporaryFilesStorage.GetIntermediateFile(CutOptionsBuilder.DefaultCutTempContainer);
            if (simpleEpisodeGroupsToConcat.Count == 1)
            {
                File.Move(simpleEpisodeGroupsToConcat.Single(), pathForConcatenatedEpisodeGroups);
            }
            else
            {
                if (this.cancellationToken.IsCancellationRequested)
                {
                    this.cancellationToken.ThrowIfCancellationRequested();
                }

                ffMpeg.Concat(pathForConcatenatedEpisodeGroups,
                    this.outputSize,
                    FFMpegCutOptions.DefaultVideoCodec,
                    FFMpegCutOptions.DefaultAudioCodec,
                    this.globalExportProgress,
                    simpleEpisodeGroupsToConcat.ToArray());
            }

            return pathForConcatenatedEpisodeGroups;
        }

        private void ConcatAndRenderTextAndImageAndTimeWarps(List<string> simpleEpisodeGroupsToConcat, FFMpeg ffMpeg, TemporaryFilesStorage temporaryFilesStorage)
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

            if (simpleEpisodeGroupsToConcat.Count == 1 &&
                !textTimeTableForConcatenatedEpisodeGroups.Any() &&
                !imagesTimeTableForConcatenatedEpisodeGroups.Any() &&
                !timeWarpForConcatenatedEpisodeGroups.Any())
            {
                File.Move(simpleEpisodeGroupsToConcat.First(), this.outputFile);
            }

            string pathForConcatenatedEpisodeGroups;
            if (simpleEpisodeGroupsToConcat.Count > 1 || textTimeTableForConcatenatedEpisodeGroups.Any() || imagesTimeTableForConcatenatedEpisodeGroups.Any())
            {
                var output = timeWarpForConcatenatedEpisodeGroups.Any()
                    ? temporaryFilesStorage.GetIntermediateFile(Path.GetExtension(this.outputFile))
                    : this.outputFile;

                ffMpeg.ConcatAndDrawImagesAndText(
                    simpleEpisodeGroupsToConcat,
                    imagesTimeTableForConcatenatedEpisodeGroups,
                    textTimeTableForConcatenatedEpisodeGroups,
                    this.outputSize,
                    output,
                    this.globalExportProgress);
                pathForConcatenatedEpisodeGroups = output;
            }
            else
            {
                pathForConcatenatedEpisodeGroups = simpleEpisodeGroupsToConcat.First();
            }

            if (timeWarpForConcatenatedEpisodeGroups.Any())
            {
                ffMpeg.ApplyTimeWarp(pathForConcatenatedEpisodeGroups, timeWarpForConcatenatedEpisodeGroups, this.outputFile, this.globalExportProgress);
            }
        }

        private string RenderGroup(FFMpeg ffMpeg, List<VideoRenderOption> renderOptionsForSimpleCut, TemporaryFilesStorage temporaryFilesStorage)
        {
            var cutOptionsBuilder = new CutOptionsBuilder(renderOptionsForSimpleCut,
                this.outputSize,
                this.globalExportProgress,
                temporaryFilesStorage,
                true);
            Parallel.ForEach(cutOptionsBuilder.CutOptions,
                cutOptions =>
                {
                    if (this.cancellationToken.IsCancellationRequested)
                    {
                        this.cancellationToken.ThrowIfCancellationRequested();
                    }

                    ffMpeg.Cut(cutOptions);
                    
                    // В процессе вырезки эпизода с параметром -ss перед источником
                    // (по другому не работает - подвисший кадр вначале эпизода в связи с тем что эпизод начинается не с ключевого кадра)
                    // мы можем получить старт эпизода раньше чем запрашивали, в связи с тем, что ключевой кадр определяется автоматом.
                    // Поэтому необходимо подкорректировать размеры эпизодов, а также всех эффектов которые привязаны ко времени.
                    this.ActualizeEpisodeDuration(ffMpeg, renderOptionsForSimpleCut, cutOptions);
                });
            var pathForGroupOfSimpleEpisodes =
                temporaryFilesStorage.GetIntermediateFile(cutOptionsBuilder.OutputExtension);
            if (cutOptionsBuilder.FilesToConcat.Count == 1)
            {
                File.Move(cutOptionsBuilder.FilesToConcat.Single(), pathForGroupOfSimpleEpisodes);
            }
            else
            {
                if (this.cancellationToken.IsCancellationRequested)
                {
                    this.cancellationToken.ThrowIfCancellationRequested();
                }
                ffMpeg.Concat(pathForGroupOfSimpleEpisodes,
                    this.outputSize,
                    "copy",
                    "copy",
                    this.globalExportProgress,
                    cutOptionsBuilder.FilesToConcat.ToArray());
            }
            return pathForGroupOfSimpleEpisodes;
        }

        private void ActualizeEpisodeDuration(FFMpeg ffMpeg, IEnumerable<VideoRenderOption> renderOptionsForSimpleCut, FFMpegCutOptions cutOptions)
        {
            var renderOptions = renderOptionsForSimpleCut.First(v =>
                Math.Abs(v.StartSecond - cutOptions.Start) < double.Epsilon &&
                Math.Abs(v.DurationSeconds - cutOptions.Duration) < double.Epsilon);
            var realDuration = ffMpeg.GetVideoInfo(cutOptions.OutputFile).Duration;
            var durationDif = realDuration.TotalSeconds - renderOptions.DurationSeconds;
            
            // Корректируем начало-конец эффектов в зависимости от рельной длительности эпизода.
            // Т.к. после вырезания текущим способом, эпизод может начинаться раньше чем задано начало.
            renderOptions.ImagesTimeTable.ForEach(v =>
            {
                v.ImageStartSecond += durationDif;
                v.ImageEndSecond += durationDif;
            });
            // Начало текста не изменяем, т.к. на данный момент там всегда 0 и он должен быть виден в течение всего видео (в будущем это может измениться).
            renderOptions.OverlayTextTimeTable.ForEach(v =>
            {
                v.EndSecond += durationDif;
            });
            renderOptions.TimeWarpSettings.ForEach(v =>
            {
                v.StartSecond += durationDif;
                v.EndSecond += durationDif;
            });
            renderOptions.DurationSeconds = realDuration.TotalSeconds;
        }
    }
}