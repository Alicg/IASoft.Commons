using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using FFMpegWrapper;
using NUnit.Framework;
using Video.Utils;

namespace VideoTests
{
    using System.Threading;
    using System.Threading.Tasks;

    [TestFixture(Description = "Тесты абстракции над FFMpeg оберткой для операций с эпизодами.")]
    public class FFMpegVideoRendererTests
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
        public void Cut1Episode_NoText_NoImages_Test()
        {
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 40, 55, null, null));
            ffmpegVideoRenderer.StartRender(OutputFolder + "Cut1Episode_NoText_NoImages.avi");
            Assert.IsTrue(File.Exists(OutputFolder + "Cut1Episode_NoText_NoImages.avi"));
        }

        [Test]
        public void Cut1Episode_ToNotExistedFolder_ExceptionExpected_Test()
        {
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 40, 55, null, null));
            ffmpegVideoRenderer.StartRender(
                OutputFolder + "NotExisted//" + "Cut1Episode_NoText_NoImages.avi",
                null,
                (d, exception) =>
                {
                    Assert.IsTrue(exception.Message.Contains("No such file or directory"));
                    Assert.Pass();
                });
        }

        [Test]
        public void Cut2Episodes_NoText_NoImages_Test()
        {
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 20, 15, null, null));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 95, 5, null, null));
            ffmpegVideoRenderer.StartRender(OutputFolder + "Cut2Episodes_NoText_NoImages.avi");
            Assert.IsTrue(File.Exists(OutputFolder + "Cut2Episodes_NoText_NoImages.avi"));
        }

        [Test]
        public void Cut2SameEpisodes_NoText_NoImages_Test()
        {
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 20, 15, null, null));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 20, 15, null, null));
            ffmpegVideoRenderer.StartRender(OutputFolder + "Cut2SameEpisodes_NoText_NoImages.avi");
            Assert.IsTrue(File.Exists(OutputFolder + "Cut2SameEpisodes_NoText_NoImages.avi"));
        }

        [Test]
        public void Cut2SameEpisodes_WithText_NoImages_PerformanceTest()
        {
            var sw = Stopwatch.StartNew();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 20, 15, "First episode", null));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 20, 15, "Second episode", null));
            ffmpegVideoRenderer.StartRender(OutputFolder + "Cut2SameEpisodes_WithText_NoImages.avi");
            sw.Stop();
            Assert.LessOrEqual(sw.Elapsed, new TimeSpan(0, 0, 20), sw.Elapsed.ToString());
            Assert.IsTrue(File.Exists(OutputFolder + "Cut2SameEpisodes_WithText_NoImages.avi"));
        }

        [Test]
        public void TestLog()
        {
            const string LogFilePath = FFMpegLogger.LogFileName;
            if (File.Exists(LogFilePath))
            {
                File.Delete(LogFilePath);
            }
            this.Cut1Episode_NoText_NoImages_Test();
            this.Cut2Episodes_NoText_NoImages_Test();
            var logText = File.ReadAllText(LogFilePath);

            var regex = new Regex("\r\n-----------*.?--------------\r\n");
            Assert.AreEqual(6, regex.Matches(logText).Count);
        }

        [Test]
        public void Cut1EpisodeWithText_FinishCallback_Test()
        {
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 40, 55, "OverlayText", null));
            double currentProgress = 0;
            ffmpegVideoRenderer.StartRender(
                OutputFolder + "Cut1Episode_WithText_NoImages_FinishCallbackTest.avi",
                (fileName, percent, currentTime, estimatedTime) =>
                {
                    currentProgress = percent;
                    Assert.GreaterOrEqual(currentTime, 0);
                    if (!double.IsNaN(estimatedTime))
                    {
                        Assert.Greater(estimatedTime, 0);
                    }
                },
                (totalDuration, fileName) =>
                {
                    Assert.AreEqual(1, currentProgress);
                });
            Assert.IsTrue(File.Exists(OutputFolder + "Cut1Episode_WithText_NoImages_FinishCallbackTest.avi"));
        }

        [Test]
        public void Cut2SameEpisodes_WithText_OneImages_FinishCallback_Test()
        {
            var sw = Stopwatch.StartNew();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            var images = new List<DrawImageTimeRecord> { new DrawImageTimeRecord(File.ReadAllBytes(SampleFiles.SamplePngImage), 100, 100, 1, 4) };
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\HDV_1626.mp4", 20, 15, "First episode", images));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\HDV_1626.mp4", 20, 15, string.Empty, new List<DrawImageTimeRecord>()));
            double currentProgress = 0;
            var outputFilePath = OutputFolder + "Cut2SameEpisodes_WithText_NoImages_FinishCallbackTest.avi";
            ffmpegVideoRenderer.StartRender(
                outputFilePath,
                (fileName, percent, currentTime, estimatedTime) =>
                {
                    currentProgress = percent;
                    Console.WriteLine("{0}% {1}sec from {2}sec", percent, currentTime, estimatedTime);
                    Assert.GreaterOrEqual(currentTime, 0);
                    if (!double.IsNaN(estimatedTime))
                    {
                        Assert.Greater(estimatedTime, 0);
                    }
                },
                (totalDuration, fileName) =>
                {
                    Assert.AreEqual(1, currentProgress);
                });
            sw.Stop();
            Assert.IsTrue(File.Exists(outputFilePath));
        }

        [Test]
        public void Cut1Episode_NoText_NoImages_Cancel_Test()
        {
            var cancelationTokenSource = new CancellationTokenSource();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer(cancelationTokenSource);
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 40, 55, null, null));
            ffmpegVideoRenderer.StartRender(
                    OutputFolder + "Cut1Episode_NoText_NoImages.avi",
                    (s, d, currentTime, estimatedTime) =>
                        {
                            if (d > 0)
                            {
                                Task.Run(() => cancelationTokenSource.Cancel());
                            }
                        },
                    (d, exception) => { Assert.IsTrue(exception is FFMpegCancelledException); });

            Assert.IsFalse(File.Exists(OutputFolder + "Cut1Episode_NoText_NoImages.avi"));
        }

        [TearDown]
        public void CleanUp()
        {
            Directory.Delete(OutputFolder, true);
        }
    }
}