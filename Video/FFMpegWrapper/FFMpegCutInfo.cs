namespace FFMpegWrapper
{
    public class FFMpegCutInfo
    {
        public FFMpegCutInfo(string inputPath, double startSecond, double endSecond)
        {
            this.InputPath = inputPath;
            this.StartSecond = startSecond;
            this.EndSecond = endSecond;
        }
        
        public string InputPath { get; }

        public int InputId { get; set; }
        
        public double StartSecond { get; }
        
        public double EndSecond { get; }
    }
}