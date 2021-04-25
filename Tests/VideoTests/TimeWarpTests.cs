namespace VideoTests
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Threading;

    using FFMpegWrapper;

    using NUnit.Framework;

    using Video.Utils;

    public class TimeWarpTests
    {
        private const string OutputFolder = "FFMpegVideoRenderer//";

        [SetUp]
        public void Init()
        {
            new VideoUtils().EnableDebugMode();
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }
        }

        [Test]
        public void Cut3Episodes_AllSlowMotion_Test()
        {
            const string Output = OutputFolder + "Cut3Episodes_AllSlowMotion.mkv";
            var sw = Stopwatch.StartNew();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(
                new VideoRenderOption(
                    SampleFiles.Helicopter_1min_48sec,
                    0,
                    15,
                    null,
                    null,
                    new List<TimeWarpRecord> { new TimeWarpRecord(2, 4, 7) }));
            ffmpegVideoRenderer.AddVideoEpisodes(
                new VideoRenderOption(
                    SampleFiles.RealInputVideoAVI,
                    1250,
                    15,
                    null,
                    null,
                    new List<TimeWarpRecord> { new TimeWarpRecord(2, 4, 7) }));
            ffmpegVideoRenderer.AddVideoEpisodes(
                new VideoRenderOption(
                    SampleFiles.LowQualityVideo,
                    140,
                    15,
                    null,
                    null,
                    new List<TimeWarpRecord> { new TimeWarpRecord(2, 4, 7) }));
            ffmpegVideoRenderer.StartRender(Output, new Size(1280, 720));
            sw.Stop();
            Thread.Sleep(3000); //чтобы лог закончил заполняться.
            Assert.IsTrue(File.Exists(Output));
        }
    }
}