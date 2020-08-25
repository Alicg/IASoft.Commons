namespace FFMpegWrapper
{
    public class FFMpegCutInfo
    {
        public FFMpegCutInfo(string inputPath, double startSecond, double endSecond, bool isMuted = false)
        {
            this.InputPath = inputPath;
            this.StartSecond = startSecond;
            this.EndSecond = endSecond;
            this.IsMuted = isMuted;
        }
        
        public FFMpegCutInfo(string videoStreamPath, string audioStreamPath, double startSecond, double endSecond, bool isMuted = false)
        {
            this.VideoStreamPath = videoStreamPath;
            this.AudioStreamPath = audioStreamPath;
            this.StartSecond = startSecond;
            this.EndSecond = endSecond;
            this.IsMuted = isMuted;
        }
        
        public string InputPath { get; }
        
        public string VideoStreamPath{get;}
        
        public string AudioStreamPath { get; }
        
        public double StartSecond { get; }
        
        public double EndSecond { get; }
        
        public bool IsMuted { get; }
    }
}