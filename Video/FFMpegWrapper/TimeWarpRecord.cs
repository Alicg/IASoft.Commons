namespace FFMpegWrapper
{
    public class TimeWarpRecord
    {
        public TimeWarpRecord(double startSecond, double endSecond, double coefficient)
        {
            this.StartSecond = startSecond;
            this.EndSecond = endSecond;
            this.Coefficient = coefficient;
        }

        public double StartSecond { get; set; }

        public double EndSecond { get; set; }

        public double Coefficient { get; }
    }
}