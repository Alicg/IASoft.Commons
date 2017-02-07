namespace FFMpegWrapper
{
    public class DrawImageTimeRecord
    {
        public DrawImageTimeRecord(byte[] imageData, int leftOffset, int topOffset, double imageStartSecond, double imageEndSecond)
        {
            this.ImageEndSecond = imageEndSecond;
            this.ImageStartSecond = imageStartSecond;
            this.TopOffset = topOffset;
            this.LeftOffset = leftOffset;
            this.ImageData = imageData;
        }

        public byte[] ImageData { get; private set; }
        public int LeftOffset { get; private set; }
        public int TopOffset { get; private set; }
        public double ImageStartSecond { get; private set; }
        public double ImageEndSecond { get; private set; }
    }
}
