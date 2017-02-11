using System;
using System.Collections.Generic;
using System.Linq;
using FFMpegWrapper;

namespace Video.Utils
{
    public class GlobalExportProgress : IGlobalExportProgress
    {
        private readonly int totalOperationsExpected;
        private readonly Action<string, double> progressChangedCallback;

        private int operationsDone;
        private double currentOperationProgress;

        private GlobalExportProgress(int totalOperationsExpected, Action<string, double> progressChangedCallback)
        {
            this.totalOperationsExpected = totalOperationsExpected;
            this.progressChangedCallback = progressChangedCallback;
        }

        public int TotalOperationsExpected => this.totalOperationsExpected;

        public double GlobalProgress => (double)this.operationsDone / this.totalOperationsExpected;

        public void SetCurrentOperationProgress(double currentProgress)
        {
            this.currentOperationProgress = currentProgress;
            this.NotifyGlobalProgress();
        }

        public void IncreaseOperationsDone()
        {
            this.operationsDone++;
            this.currentOperationProgress = 0;
            this.NotifyGlobalProgress();
        }

        private void NotifyGlobalProgress()
        {
            var globalProgress = (this.operationsDone + this.currentOperationProgress) / this.totalOperationsExpected;
            this.progressChangedCallback?.Invoke(null, globalProgress);
        }

        public static GlobalExportProgress Empty => new GlobalExportProgress(0, null);

        public static GlobalExportProgress BuildFromRenderOptions(ICollection<VideoRenderOption> videoRenderOptions, Action<string, double> progressChangedCallback)
        {
            // по одной операции для вырезания каждого эпизода.
            var totalOperationsExpected = videoRenderOptions.Count;

            // по одной операции для каждой отрисовки текста на эпизоде.
            totalOperationsExpected += videoRenderOptions.Count(v => !string.IsNullOrEmpty(v.OverlayText));

            // по одной операции для каждой отрисовки штрихов на эпизоде.
            totalOperationsExpected += videoRenderOptions.Count(v => v.ImagesTimeTable != null && v.ImagesTimeTable.Any());

            if(videoRenderOptions.Count > 1)
            {
                // один раз склеить эпизоды.
                totalOperationsExpected += 1;
            }

            return new GlobalExportProgress(totalOperationsExpected, progressChangedCallback);
        }
    }
}