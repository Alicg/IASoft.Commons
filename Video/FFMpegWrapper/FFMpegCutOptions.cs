using System.Drawing;

namespace FFMpegWrapper
{
    public class FFMpegCutOptions
    {
        public const string DefaultVideoCodec = "libx264";
        public const string DefaultAudioCodec = "aac";
        
        private FFMpegCutOptions(string inputFile, string outputFile, double start, double duration, string videoCodec, string audioCodec, IGlobalExportProgress globalExportProgress)
        {
            this.InputFile = inputFile;
            this.OutputFile = outputFile;
            this.Start = start;
            this.Duration = duration;
            this.GlobalExportProgress = globalExportProgress;
            this.VideoCodec = videoCodec;
            this.AudioCodec = audioCodec;
            this.SimpleMode = true;
        }

        private FFMpegCutOptions(
            string inputFile,
            string outputFile,
            double start,
            double duration,
            IGlobalExportProgress globalExportProgress,
            Size outputSize,
            string videoCodec,
            string audioCodec) : this(inputFile, outputFile, start, duration, videoCodec, audioCodec, globalExportProgress)
        {
            this.OutputSize = outputSize;
            this.SimpleMode = false;
        }

        public string InputFile { get; }

        public string OutputFile { get; }

        public double Start { get; }

        public double Duration { get; }

        public IGlobalExportProgress GlobalExportProgress { get; }

        public Size OutputSize { get; }

        public string VideoCodec { get; }

        public string AudioCodec { get; }
        
        public bool SimpleMode { get; }

        public static FFMpegCutOptions BuildSimpleCatOptions(
            string inputFile,
            string outputFile,
            double start,
            double duration,
            IGlobalExportProgress globalExportProgress)
        {
            return new FFMpegCutOptions(inputFile, outputFile, start, duration, "copy", "copy", globalExportProgress);
        }

        public static FFMpegCutOptions BuildCatOptionsWithConvertations(
            string inputFile,
            string outputFile,
            double start,
            double duration,
            IGlobalExportProgress globalExportProgress,
            Size outputSize)
        {
            return new FFMpegCutOptions(inputFile, outputFile, start, duration, globalExportProgress, outputSize, DefaultVideoCodec, DefaultAudioCodec);
        }

        public static FFMpegCutOptions BuildCatOptionsWithConvertations(
            string inputFile,
            string outputFile,
            double start,
            double duration,
            IGlobalExportProgress globalExportProgress,
            Size outputSize,
            string videoCodec,
            string audioCodec)
        {
            return new FFMpegCutOptions(inputFile, outputFile, start, duration, globalExportProgress, outputSize, videoCodec, audioCodec);
        }

        public FFMpegCutOptions CloneWithOtherOutput(string outputFile)
        {
            return new FFMpegCutOptions(
                this.InputFile,
                outputFile,
                this.Start,
                this.Duration,
                this.GlobalExportProgress,
                this.OutputSize,
                this.VideoCodec,
                this.AudioCodec);
        }
    }
}