using System;
using System.Collections.Generic;
using System.Linq;
using FFMpegWrapper;

namespace Video.Utils
{
    public class GlobalExportProgress : IGlobalExportProgress
    {
        private readonly int totalOperationsExpected;

        /// <summary>
        /// Оповещает о том, что видео с именем STRING было полностью экспортировано на double1 процентов, или на double2 секунд из double3 общих расчётных секунд.
        /// </summary>
        private readonly Action<string, double, double, double> progressChangedCallback;

        private int operationsDone;

        /// <summary>
        /// Число секунд, прошедших с начала выполнения текущей операции.
        /// </summary>
        private double currentSeconds;

        /// <summary>
        /// Общее число секунд, прошедшее с начала экспорта.
        /// </summary>
        private double totalCurrentSeconds;

        private double currentOperationProgress;

        private GlobalExportProgress(int totalOperationsExpected, Action<string, double, double, double> progressChangedCallback)
        {
            this.totalOperationsExpected = totalOperationsExpected;
            this.progressChangedCallback = progressChangedCallback;
        }

        public int TotalOperationsExpected => this.totalOperationsExpected;

        public double GlobalProgress => (double)this.operationsDone / this.totalOperationsExpected;

        /// <param name="currentSeconds">Число секунд, прошедших с начала выполнения текущей операции.</param>
        /// <param name="totalEstimatedSeconds">Рассчетное число секунд, требуемое для полного выполнения текущей операции.</param>
        public void SetCurrentOperationProgress(double currentSeconds, double totalEstimatedSeconds)
        {
            this.currentSeconds = currentSeconds;

            if (currentSeconds > totalEstimatedSeconds)
            {
                this.currentOperationProgress = 1;
            }
            else
            {
                // FFMpeg может долго отдавать нулевой прогресс, но если операция началась будем отдавать хотя бы о 10% текущей операции.
                this.currentOperationProgress = Math.Max(0.1, currentSeconds / totalEstimatedSeconds);
            }

            this.NotifyGlobalProgress();
        }

        public void IncreaseOperationsDone()
        {
            this.operationsDone++;
            this.totalCurrentSeconds += this.currentSeconds;
            this.currentSeconds = 0;
            this.currentOperationProgress = 0;
            this.NotifyGlobalProgress();
        }

        private void NotifyGlobalProgress()
        {
            var globalProgress = (this.operationsDone + this.currentOperationProgress) / this.totalOperationsExpected;
            var totalEstimatedTime = globalProgress > 0.1 ? (this.totalCurrentSeconds + this.currentSeconds) / globalProgress : double.NaN;
            this.progressChangedCallback?.Invoke(null, globalProgress, this.totalCurrentSeconds + this.currentSeconds, totalEstimatedTime);
        }

        public static GlobalExportProgress Empty => new GlobalExportProgress(0, null);

        public static GlobalExportProgress BuildFromRenderOptions(ICollection<VideoRenderOption> videoRenderOptions, Action<string, double, double, double> progressChangedCallback)
        {
            // по одной операции для вырезания каждого эпизода.
            var totalOperationsExpected = videoRenderOptions.Count;

            // по одной операции для каждой отрисовки текста на эпизоде.
            totalOperationsExpected += videoRenderOptions.Count(v => !string.IsNullOrEmpty(v.OverlayText));

            // по одной операции для каждой отрисовки штрихов на эпизоде.
            totalOperationsExpected += videoRenderOptions.Count(v => v.ImagesTimeTable != null && v.ImagesTimeTable.Any());

            if (videoRenderOptions.Count > 1)
            {
                // один раз склеить эпизоды.
                totalOperationsExpected += 1;
            }

            return new GlobalExportProgress(totalOperationsExpected, progressChangedCallback);
        }
    }
}