namespace FFMpegWrapper
{
    using System.Drawing;

    internal class Pixel
    {
        public Pixel(int startSecond, int endSecond, int x, int y, Color color)
        {
            this.X = x;
            this.Y = y;
            this.StartSecond = startSecond;
            this.EndSecond = endSecond;
            this.Color = color;
        }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int StartSecond { get; private set; }
        public int EndSecond { get; private set; }
        public Color Color { get; private set; }
    }
}
