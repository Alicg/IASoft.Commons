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
            List<TextTimeRecord> overlayText,
            List<DrawImageTimeRecord> imagesTimeTable,
            List<TimeWarpRecord> timeWarpSettings)
        {
            this.OverlayTextTimeTable = overlayText;
            this.StartSecond = startSecond;
            this.DurationSeconds = durationSeconds;
            this.FilePath = filePath;
            this.ImagesTimeTable = imagesTimeTable;
            this.TimeWarpSettings = timeWarpSettings;
        }

        public VideoRenderOption(string filePath, double startSecond, double durationSeconds, List<TextTimeRecord> overlayText, List<DrawImageTimeRecord> imagesTimeTable)
            : this(filePath, startSecond, durationSeconds, overlayText, imagesTimeTable, new List<TimeWarpRecord>())
        {
        }

        public VideoRenderOption(string filePath, double startSecond, double durationSeconds, List<TextTimeRecord> overlayText)
            : this(filePath, startSecond, durationSeconds, overlayText, new List<DrawImageTimeRecord>())
        {
        }

        public VideoRenderOption(string filePath, double startSecond, double durationSeconds)
            : this(filePath, startSecond, durationSeconds, new List<TextTimeRecord>())
        {
        }

        public string FilePath { get; }

        public double StartSecond { get; }

        public double DurationSeconds { get; set; }

        public List<TextTimeRecord> OverlayTextTimeTable { get; }

        public List<DrawImageTimeRecord> ImagesTimeTable { get; }

        public List<TimeWarpRecord> TimeWarpSettings { get; }
    }
}