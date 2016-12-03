namespace FFMpegWrapper
{
    using System.Drawing;

    public class Pixel
    {
        public Pixel(int startSecond, int endSecond, int x, int y, Color color)
        {
            X = x;
            Y = y;
            StartSecond = startSecond;
            EndSecond = endSecond;
            Color = color;
        }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int StartSecond { get; private set; }
        public int EndSecond { get; private set; }
        public Color Color { get; private set; }
    }
}
