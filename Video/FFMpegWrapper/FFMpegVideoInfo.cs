using System;

namespace FFMpegWrapper
{
    public class FFMpegVideoInfo
    {
        public FFMpegVideoInfo(int width, int height, TimeSpan duration)
        {
            this.Height = height;
            this.Width = width;
            this.Duration = duration;
        }

        public int Width { get; }
        public int Height { get; }
        public TimeSpan Duration { get; }
    }
}
