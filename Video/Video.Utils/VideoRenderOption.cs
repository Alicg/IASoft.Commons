namespace Video.Utils
{
    using System.Collections.Generic;

    using FFMpegWrapper;

    public class VideoRenderOption
    {
        public VideoRenderOption(string filePath, double startSecond, double durationSeconds, string overlayText, List<DrawImageTimeRecord> imagesTimeTable)
        {
            this.OverlayText = overlayText;
            this.StartSecond = startSecond;
            this.DurationSeconds = durationSeconds;
            this.FilePath = filePath;
            this.ImagesTimeTable = imagesTimeTable;
        }

        public string FilePath { get; private set; }

        public double StartSecond { get; private set; }

        public double DurationSeconds { get; }

        public string OverlayText { get; private set; }

        public List<DrawImageTimeRecord> ImagesTimeTable { get; private set; }
    }
}
