namespace Video.Utils
{
    using System.Collections.Generic;

    using FFMpegWrapper;

    public class VideoRenderOption
    {
        public VideoRenderOption(string filePath, double start, double end, string overlayText, List<DrawImageTimeRecord> imagesTimeTable)
        {
            OverlayText = overlayText;
            End = end;
            Start = start;
            FilePath = filePath;
            ImagesTimeTable = imagesTimeTable;
        }

        public string FilePath { get; private set; }
        public double Start { get; private set; }
        public double End { get; private set; }
        public string OverlayText { get; private set; }
        public List<DrawImageTimeRecord> ImagesTimeTable { get; private set; }
    }
}
