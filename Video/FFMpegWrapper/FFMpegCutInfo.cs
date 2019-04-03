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
        
        public FFMpegCutInfo(string videoStreamPath, string audioStreamPath, double startSecond, double endSecond)
        {
            this.VideoStreamPath = videoStreamPath;
            this.AudioStreamPath = audioStreamPath;
            this.StartSecond = startSecond;
            this.EndSecond = endSecond;
        }
        
        public string InputPath { get; }
        
        public string VideoStreamPath{get;}
        
        public string AudioStreamPath { get; }
        
        public double StartSecond { get; }
        
        public double EndSecond { get; }
    }
}