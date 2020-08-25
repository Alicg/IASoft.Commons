namespace Video.Utils
{
    using System.Collections.Generic;

    using FFMpegWrapper;

    public class VideoRenderOption
    {
        public VideoRenderOption(
            string videoStreamPath,
            string audioStreamPath,
            double startSecond,
            double durationSeconds,
            List<TextTimeRecord> overlayText,
            List<DrawImageTimeRecord> imagesTimeTable,
            List<TimeWarpRecord> timeWarpSettings,
            bool isMuted = false)
        {
            this.VideoStreamPath = videoStreamPath;
            this.AudioStreamPath = audioStreamPath;
            this.OverlayTextTimeTable = overlayText;
            this.StartSecond = startSecond;
            this.DurationSeconds = durationSeconds;
            this.ImagesTimeTable = imagesTimeTable;
            this.TimeWarpSettings = timeWarpSettings;
            this.IsMuted = isMuted;
        }

        public VideoRenderOption(string videoStreamPath, string audioStreamPath, double startSecond, double durationSeconds, bool isMuted = false)
            : this(videoStreamPath, audioStreamPath, startSecond, durationSeconds, new List<TextTimeRecord>(), new List<DrawImageTimeRecord>(), new List<TimeWarpRecord>(), isMuted)
        {
        }
        
        public VideoRenderOption(
            string filePath,
            double startSecond,
            double durationSeconds,
            List<TextTimeRecord> overlayText,
            List<DrawImageTimeRecord> imagesTimeTable,
            List<TimeWarpRecord> timeWarpSettings,
            bool isMuted = false)
        {
            this.FilePath = filePath;
            this.OverlayTextTimeTable = overlayText;
            this.StartSecond = startSecond;
            this.DurationSeconds = durationSeconds;
            this.ImagesTimeTable = imagesTimeTable;
            this.TimeWarpSettings = timeWarpSettings;
            this.IsMuted = isMuted;
        }

        public VideoRenderOption(string filePath, double startSecond, double durationSeconds, List<TextTimeRecord> overlayText, List<DrawImageTimeRecord> imagesTimeTable, bool isMuted = false)
            : this(filePath, startSecond, durationSeconds, overlayText, imagesTimeTable, new List<TimeWarpRecord>(), isMuted)
        {
        }

        public VideoRenderOption(string filePath, double startSecond, double durationSeconds, List<TextTimeRecord> overlayText, bool isMuted = false)
            : this(filePath, startSecond, durationSeconds, overlayText, new List<DrawImageTimeRecord>(), isMuted)
        {
        }

        public VideoRenderOption(string filePath, double startSecond, double durationSeconds, bool isMuted = false)
            : this(filePath, startSecond, durationSeconds, new List<TextTimeRecord>(), isMuted)
        {
        }

        public string FilePath { get; }
        
        public string VideoStreamPath{get;}
        
        public string AudioStreamPath { get; }

        public double StartSecond { get; }

        public double DurationSeconds { get; set; }
        
        public bool IsMuted { get; }

        public List<TextTimeRecord> OverlayTextTimeTable { get; }

        public List<DrawImageTimeRecord> ImagesTimeTable { get; }

        public List<TimeWarpRecord> TimeWarpSettings { get; }
    }
}