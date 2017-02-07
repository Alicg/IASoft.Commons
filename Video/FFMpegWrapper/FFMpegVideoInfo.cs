namespace FFMpegWrapper
{
    public class FFMpegVideoInfo
    {
        public FFMpegVideoInfo(int width, int height)
        {
            this.Height = height;
            this.Width = width;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}
