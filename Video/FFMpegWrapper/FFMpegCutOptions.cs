using System.Drawing;
using System.Linq;

namespace FFMpegWrapper
{
    using System.Collections.Generic;

    public class FFMpegCutOptions
    {
        public const string DefaultVideoCodec = "libx264";

        public const string DefaultAudioCodec = "aac";

        private FFMpegCutOptions(
            string inputFile,
            string outputFile,
            double start,
            double duration,
            string videoCodec,
            string audioCodec,
            IGlobalExportProgress globalExportProgress)
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
            string audioCodec,
            IEnumerable<TextTimeRecord> overlayText,
            IEnumerable<DrawImageTimeRecord> imagesTimeTable,
            IEnumerable<TimeWarpRecord> timeWarps)
            : this(inputFile, outputFile, start, duration, videoCodec, audioCodec, globalExportProgress)
        {
            this.OutputSize = outputSize;
            this.SimpleMode = false;
            this.OverlayText = overlayText?.ToList();
            this.ImagesTimeTable = imagesTimeTable?.ToList();
            this.TimeWarps = timeWarps?.ToList();
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

        public List<TextTimeRecord> OverlayText { get; }

        public List<DrawImageTimeRecord> ImagesTimeTable { get; }

        public List<TimeWarpRecord> TimeWarps { get; }

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
            return new FFMpegCutOptions(inputFile, outputFile, start, duration, globalExportProgress, outputSize, DefaultVideoCodec, DefaultAudioCodec, null, null, null);
        }

        public static FFMpegCutOptions BuildCatOptionsWithConvertations(
            string inputFile,
            string outputFile,
            double start,
            double duration,
            IGlobalExportProgress globalExportProgress,
            Size outputSize,
            List<TextTimeRecord> overlayText,
            List<DrawImageTimeRecord> images,
            List<TimeWarpRecord> timeWarps)
        {
            return new FFMpegCutOptions(
                inputFile,
                outputFile,
                start,
                duration,
                globalExportProgress,
                outputSize,
                DefaultVideoCodec,
                DefaultAudioCodec,
                overlayText,
                images,
                timeWarps);
        }

        public static FFMpegCutOptions BuildCatOptionsWithConvertations(
            string inputFile,
            string outputFile,
            double start,
            double duration,
            IGlobalExportProgress globalExportProgress,
            Size outputSize,
            string videoCodec,
            string audioCodec,
            List<TextTimeRecord> overlayText,
            List<DrawImageTimeRecord> images,
            List<TimeWarpRecord> timeWarps)
        {
            return new FFMpegCutOptions(
                inputFile,
                outputFile,
                start,
                duration,
                globalExportProgress,
                outputSize,
                videoCodec,
                audioCodec,
                overlayText,
                images,
                timeWarps);
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
                this.AudioCodec,
                this.OverlayText,
                this.ImagesTimeTable,
                this.TimeWarps);
        }
    }
}