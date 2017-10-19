using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 40, 55));
            ffmpegVideoRenderer.StartRender(OutputFolder + "Cut1Episode_NoText_NoImages.mp4");
            Assert.IsTrue(File.Exists(OutputFolder + "Cut1Episode_NoText_NoImages.mp4"));
        }

        [Test]
        public void Cut1Episode_ToNotExistedFolder_ExceptionExpected_Test()
        {
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 40, 55));
            ffmpegVideoRenderer.StartRender(
                OutputFolder + "NotExisted//" + "Cut1Episode_NoText_NoImages.mp4",
                null,
                (d, exception) =>
                {
                    Assert.IsTrue(exception is DirectoryNotFoundException);
                    Assert.Pass();
                });
        }

        [Test]
        public void Cut2Episodes_NoText_NoImages_Test()
        {
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 20, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 95, 5));
            ffmpegVideoRenderer.StartRender(OutputFolder + "Cut2Episodes_NoText_NoImages.mp4");
            Assert.IsTrue(File.Exists(OutputFolder + "Cut2Episodes_NoText_NoImages.mp4"));
        }

        [Test]
        public void Cut5Episodes_SameSource_NoText_NoImages_PerformanceTest()
        {
            const string Output = OutputFolder + "Cut5Episodes_NoText_NoImages.mkv";
            var sw = Stopwatch.StartNew(); 
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\Гандбол. ЛЧ. Муж. 4-й тур. Монпелье-Мишки. Рутрекер орг. 15.10.2016.avi", 1250, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\Гандбол. ЛЧ. Муж. 4-й тур. Монпелье-Мишки. Рутрекер орг. 15.10.2016.avi", 750, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\Гандбол. ЛЧ. Муж. 4-й тур. Монпелье-Мишки. Рутрекер орг. 15.10.2016.avi", 550, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\Гандбол. ЛЧ. Муж. 4-й тур. Монпелье-Мишки. Рутрекер орг. 15.10.2016.avi", 350, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\Гандбол. ЛЧ. Муж. 4-й тур. Монпелье-Мишки. Рутрекер орг. 15.10.2016.avi", 1750, 15));
            ffmpegVideoRenderer.StartRender(Output);
            sw.Stop();
            Assert.IsTrue(File.Exists(Output));
            Assert.LessOrEqual(sw.ElapsedMilliseconds, 3000);
        }

        [Test]
        public void Cut5Episodes_SameSource_NoText_NoImages_ProgressTest()
        {
            const string Output = OutputFolder + "Cut5Episodes_NoText_NoImages.mkv";
            var sw = Stopwatch.StartNew(); 
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\Гандбол. ЛЧ. Муж. 4-й тур. Монпелье-Мишки. Рутрекер орг. 15.10.2016.avi", 1250, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\Гандбол. ЛЧ. Муж. 4-й тур. Монпелье-Мишки. Рутрекер орг. 15.10.2016.avi", 750, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\Гандбол. ЛЧ. Муж. 4-й тур. Монпелье-Мишки. Рутрекер орг. 15.10.2016.avi", 550, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\Гандбол. ЛЧ. Муж. 4-й тур. Монпелье-Мишки. Рутрекер орг. 15.10.2016.avi", 350, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\Гандбол. ЛЧ. Муж. 4-й тур. Монпелье-Мишки. Рутрекер орг. 15.10.2016.avi", 1750, 15));

            double previousProgress = 0;
            ffmpegVideoRenderer.StartRender(
                Output,
                (s, d, arg3, arg4) =>
                {
                    Assert.GreaterOrEqual(d, previousProgress);
                    Assert.GreaterOrEqual(d, 0);
                    Assert.LessOrEqual(d, 1);
                    previousProgress = d;
                });
            sw.Stop();
            Thread.Sleep(1000);//чтобы лог закончил заполняться.
            Assert.IsTrue(File.Exists(Output));
        }

        [Test]
        public void Cut3Episodes_DifferentFormat_NoText_NoImages_Test()
        {
            const string Output = OutputFolder + "Cut3Episodes_DifferentFormat_NoText_NoImages.mkv";
            var sw = Stopwatch.StartNew();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\Байдарки 07.08.2016\GOPR0041.MP4", 0, 15, null, null));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\Гандбол. ЛЧ. Муж. 4-й тур. Монпелье-Мишки. Рутрекер орг. 15.10.2016.avi", 1250, 15, null, null));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\LRF -Basement- taurė 2017 Klaipėdos -Dragūnas- - HC Vilnius.mp4", 140, 15, null, null));
            ffmpegVideoRenderer.StartRender(Output, new Size(1280, 720));
            sw.Stop();
            Thread.Sleep(1000);//чтобы лог закончил заполняться.
            Assert.IsTrue(File.Exists(Output));
        }

        [Test]
        public void Cut2SameEpisodes_NoText_NoImages_Test()
        {
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 20, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 20, 15));
            ffmpegVideoRenderer.StartRender(OutputFolder + "Cut2SameEpisodes_NoText_NoImages.mp4");
            Assert.IsTrue(File.Exists(OutputFolder + "Cut2SameEpisodes_NoText_NoImages.mp4"));
        }

        [Test]
        public void Cut2SameEpisodes_WithText_NoImages_PerformanceTest()
        {
            var sw = Stopwatch.StartNew();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 20, 15, "First episode"));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 20, 15, "Second episode"));
            ffmpegVideoRenderer.StartRender(OutputFolder + "Cut2SameEpisodes_WithText_NoImages.mp4");
            sw.Stop();
            Assert.LessOrEqual(sw.Elapsed, new TimeSpan(0, 0, 20), sw.Elapsed.ToString());
            Assert.IsTrue(File.Exists(OutputFolder + "Cut2SameEpisodes_WithText_NoImages.mp4"));
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
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec, 40, 55, "OverlayText"));
            double currentProgress = 0;
            ffmpegVideoRenderer.StartRender(
                OutputFolder + "Cut1Episode_WithText_NoImages_FinishCallbackTest.mp4",
                (fileName, percent, currentTime, estimatedTime) =>
                {
                    currentProgress = percent;
                    Assert.GreaterOrEqual(currentTime, 0);
                },
                (totalDuration, fileName) =>
                {
                    Assert.AreEqual(1, currentProgress);
                });
            Assert.IsTrue(File.Exists(OutputFolder + "Cut1Episode_WithText_NoImages_FinishCallbackTest.mp4"));
        }

        [Test]
        public void Cut2SameEpisodes_WithText_OneImages_FinishCallback_Test()
        {
            var sw = Stopwatch.StartNew();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            var images = new List<DrawImageTimeRecord> { new DrawImageTimeRecord(File.ReadAllBytes(SampleFiles.SamplePngImage), 100, 100, 1, 4) };
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\HDV_1626.mp4", 20, 15, "First episode", images));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\HDV_1626.mp4", 300, 15, "Third episode", new List<DrawImageTimeRecord>()));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\HDV_1626.mp4", 500, 15, string.Empty, new List<DrawImageTimeRecord>()));
            double currentProgress = 0;
            var outputFilePath = OutputFolder + "Cut2SameEpisodes_WithText_NoImages_FinishCallbackTest.mkv";
            ffmpegVideoRenderer.StartRender(
                outputFilePath,
                (fileName, percent, currentTime, estimatedTime) =>
                {
                    currentProgress = percent;
                    Console.WriteLine("{0}% {1}sec from {2}sec", percent, currentTime, estimatedTime);
                    Assert.GreaterOrEqual(currentTime, 0);
                },
                (totalDuration, exception) =>
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
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\HDV_1626.mp4", 40, 55));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\HDV_1626.mp4", 40, 55));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\HDV_1626.mp4", 40, 55));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(@"M:\SVA.Videos\HDV_1626.mp4", 40, 55));
            ffmpegVideoRenderer.StartRender(
                OutputFolder + "Cut1Episode_NoText_NoImages.mp4",
                (s, d, currentTime, estimatedTime) =>
                {
                    if (d > 0)
                    {
                        Task.Run(() => cancelationTokenSource.Cancel());
                    }
                },
                (d, exception) =>
                {
                    var aggregateException = exception as AggregateException;
                    var isAggregateAndAllInnerAreCancelledExceptions = aggregateException != null && aggregateException.InnerExceptions.All(e => e is FFMpegCancelledException);
                    Assert.IsTrue(exception is FFMpegCancelledException || isAggregateAndAllInnerAreCancelledExceptions, exception.ToString());
                });

            Assert.IsFalse(File.Exists(OutputFolder + "Cut1Episode_NoText_NoImages.mp4"));
        }

        [Test]
        public void Cut1Episode_NotExistedInput_ExceptionTest()
        {
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(string.Empty, 40, 55));
            ffmpegVideoRenderer.StartRender(
                OutputFolder + "Cut1Episode_NoText_NoImages.mp4",
                (s, d, currentTime, estimatedTime) =>
                    {
                    },
                (d, exception) =>
                    {
                        var aggregateException = exception as AggregateException;
                        var firstException = aggregateException.InnerException;
                        Assert.IsTrue(firstException.Message.Contains("No such file or directory"));
                    });
            Assert.IsFalse(File.Exists(OutputFolder + "Cut1Episode_NoText_NoImages.mp4"));
        }



        [TearDown]
        public void CleanUp()
        {
            Directory.Delete(OutputFolder, true);
        }
    }
}