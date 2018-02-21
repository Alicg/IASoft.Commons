using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FFMpegWrapper;

namespace Video.Utils
{
    public class GlobalExportProgress : IGlobalExportProgress
    {
        private readonly ConcurrentDictionary<int, double> activeOperationsProgress = new ConcurrentDictionary<int, double>();
        private readonly int totalOperationsExpected;
        private DateTime exportStartedAt;

        /// <summary>
        /// Оповещает о том, что видео с именем STRING было полностью экспортировано на double1 процентов, или на double2 секунд из double3 общих расчётных секунд.
        /// </summary>
        private readonly Action<string, double, double, double> progressChangedCallback;

        private int operationsDone;

        private GlobalExportProgress(int totalOperationsExpected, Action<string, double, double, double> progressChangedCallback)
        {
            this.totalOperationsExpected = totalOperationsExpected;
            this.progressChangedCallback = progressChangedCallback;
        }

        public int TotalOperationsExpected => this.totalOperationsExpected;

        public double GlobalProgress => (double)this.operationsDone / this.totalOperationsExpected;

        public void StartExport()
        {
            this.exportStartedAt = DateTime.Now;
        }

        /// <param name="currentSeconds">Число секунд, обработанных текущей операцией.</param>
        /// <param name="totalEstimatedSeconds">Общее число секунд, которые нужно обработать текущей операции.</param>
        public void SetCurrentOperationProgress(double currentSeconds, double totalEstimatedSeconds, int processId)
        {
            if (!this.activeOperationsProgress.ContainsKey(processId))
            {
                this.activeOperationsProgress.TryAdd(processId, Math.Min(1, Math.Max(0.1, currentSeconds / totalEstimatedSeconds)));
            }
            else
            {
                this.activeOperationsProgress[processId] = Math.Min(1, Math.Max(0.1, currentSeconds / totalEstimatedSeconds));
            }

            this.NotifyGlobalProgress();
        }

        public void IncreaseOperationsDone(int processId, int count = 1)
        {
            Interlocked.Add(ref this.operationsDone, count);
            this.activeOperationsProgress.TryRemove(processId, out double _);
            this.NotifyGlobalProgress();
        }

        private void NotifyGlobalProgress()
        {
            var values = this.activeOperationsProgress.Values;
            var activeOperationsCompleted = values.Any() ? values.Aggregate((t, c) => t + c) : 0;
            var globalProgress = (this.operationsDone + activeOperationsCompleted) / this.totalOperationsExpected;
            this.progressChangedCallback?.Invoke(null, globalProgress, (DateTime.Now - this.exportStartedAt).TotalSeconds, 0);
        }

        public static GlobalExportProgress Empty => new GlobalExportProgress(0, null);

        public static GlobalExportProgress BuildFromRenderOptionsPreEffect(ICollection<VideoRenderOption> videoRenderOptions, Action<string, double, double, double> progressChangedCallback)
        {
            // по одной операции для вырезания каждого эпизода.
            var totalOperationsExpected = videoRenderOptions.Count;

            // по одной операции для каждого эпизода с текстом.
            totalOperationsExpected += videoRenderOptions.Count(v => v.OverlayTextTimeTable != null && v.OverlayTextTimeTable.Any());

            // по одной операции для каждого эпизода со штрихами.
            totalOperationsExpected += videoRenderOptions.Count(v => v.ImagesTimeTable != null && v.ImagesTimeTable.Any());

            // по одной операции для каждого эффекта времени.
            totalOperationsExpected += videoRenderOptions.SelectMany(v => v.TimeWarpSettings ?? new List<TimeWarpRecord>()).Count();

            if (videoRenderOptions.Count > 1)
            {
                // один раз склеить и конвертировать эпизоды в конечный формат.
                totalOperationsExpected += 2;
            }

            var progress = new GlobalExportProgress(totalOperationsExpected, progressChangedCallback);
            progress.StartExport();
            return progress;
        }

        public static GlobalExportProgress BuildFromRenderOptionsPostEffect(ICollection<VideoRenderOption> videoRenderOptions, Action<string, double, double, double> progressChangedCallback)
        {
            // по одной операции для вырезания каждого эпизода.
            var totalOperationsExpected = videoRenderOptions.Count;

            var differentSourcesCount = videoRenderOptions.Select(v => v.FilePath).GroupBy(v => v).Count(v => v.Count() > 1);
            // склеить эпизоды в группы одного формата (если таких эпизодов несколько).
            totalOperationsExpected += differentSourcesCount;

            var totalOperationsForConcat = 0;
            if (differentSourcesCount > 1)
            {
                // один раз склеить все группы разных форматов.
                totalOperationsForConcat = 1;
            }

            var totalOverlayTextRecords = videoRenderOptions
                .SelectMany(v => v.OverlayTextTimeTable ?? new List<TextTimeRecord>()).Count();
            
            var totalOverlayImageRecords = videoRenderOptions
                .SelectMany(v => v.ImagesTimeTable ?? new List<DrawImageTimeRecord>()).Count();
            
            var totalOperationsForFilter = Math.Max(totalOperationsForConcat, Math.Max(totalOverlayTextRecords, totalOverlayImageRecords)) * 2;

            totalOperationsExpected += totalOperationsForFilter;

            // по одной операции для каждого эффекта времени.
            totalOperationsExpected += videoRenderOptions.Count(v => v.TimeWarpSettings != null && v.TimeWarpSettings.Any());

            var progress = new GlobalExportProgress(totalOperationsExpected, progressChangedCallback);
            progress.StartExport();
            return progress;
        }
    }
}