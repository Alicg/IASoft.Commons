namespace FFMpegWrapper
{
    public class FFMpegVideoInfo
    {
        public FFMpegVideoInfo(int width, int height)
        {
            Height = height;
            Width = width;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}
