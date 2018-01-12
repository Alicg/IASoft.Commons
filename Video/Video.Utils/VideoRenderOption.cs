namespace Video.Utils
{
    using System.Collections.Generic;

    using FFMpegWrapper;

    public class VideoRenderOption
    {
        public VideoRenderOption(
            string filePath,
            double startSecond,
            double durationSeconds,
            string overlayText,
            List<DrawImageTimeRecord> imagesTimeTable,
            List<TimeWarpRecord> timeWarpSettings)
        {
            this.OverlayText = overlayText;
            this.StartSecond = startSecond;
            this.DurationSeconds = durationSeconds;
            this.FilePath = filePath;
            this.ImagesTimeTable = imagesTimeTable;
            this.TimeWarpSettings = timeWarpSettings;
        }

        public VideoRenderOption(string filePath, double startSecond, double durationSeconds, string overlayText, List<DrawImageTimeRecord> imagesTimeTable)
        {
            this.OverlayText = overlayText;
            this.StartSecond = startSecond;
            this.DurationSeconds = durationSeconds;
            this.FilePath = filePath;
            this.ImagesTimeTable = imagesTimeTable;
        }

        public VideoRenderOption(string filePath, double startSecond, double durationSeconds, string overlayText)
            : this(filePath, startSecond, durationSeconds, overlayText, new List<DrawImageTimeRecord>())
        {
        }

        public VideoRenderOption(string filePath, double startSecond, double durationSeconds)
            : this(filePath, startSecond, durationSeconds, string.Empty)
        {
        }

        public string FilePath { get; }

        public double StartSecond { get; }

        public double DurationSeconds { get; }

        public string OverlayText { get; }

        public List<DrawImageTimeRecord> ImagesTimeTable { get; }

        public List<TimeWarpRecord> TimeWarpSettings { get; }
    }
}