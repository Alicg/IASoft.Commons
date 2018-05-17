using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FFMpegExecutable;
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
                    Assert.IsTrue(exception is FFMpegException);
                    Assert.IsTrue(exception.Message.Contains("No such file or directory"));
                    Assert.Pass();
                });
        }

        [Test]
        public void Cut6Episodes_WithText_NoImages_DIVX_Test()
        {
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec,
                20,
                15,
                new List<TextTimeRecord> {new TextTimeRecord("First", 0, 15)}));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec,
                20,
                15,
                new List<TextTimeRecord> {new TextTimeRecord("Second", 0, 15)}));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec,
                20,
                15,
                new List<TextTimeRecord> {new TextTimeRecord("Third", 0, 15)}));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec,
                20,
                15,
                new List<TextTimeRecord> {new TextTimeRecord("Fourth", 0, 15)}));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec,
                20,
                15,
                new List<TextTimeRecord> {new TextTimeRecord("Fifth", 0, 15)}));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec,
                95,
                5,
                new List<TextTimeRecord> {new TextTimeRecord("Sixth", 0, 5)}));
            var stopWatch = Stopwatch.StartNew();
            ffmpegVideoRenderer.StartRender(OutputFolder + "Cut2Episodes_WithText_NoImages.mp4");
            Console.Write(stopWatch.Elapsed);
            Assert.IsTrue(File.Exists(OutputFolder + "Cut2Episodes_WithText_NoImages.mp4"));
        }

        [Test]
        public void Cut6Episodes_WithText_NoImages_MP4_Test()
        {
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoMP4,
                20,
                15,
                new List<TextTimeRecord> {new TextTimeRecord("First", 0, 15)}));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoMP4,
                20,
                15,
                new List<TextTimeRecord> {new TextTimeRecord("Second", 0, 15)}));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoMP4,
                1200,
                15,
                new List<TextTimeRecord> {new TextTimeRecord("Third", 0, 15)}));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoMP4,
                1000,
                15,
                new List<TextTimeRecord> {new TextTimeRecord("Fourth", 0, 15)}));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoMP4,
                1200,
                15,
                new List<TextTimeRecord> {new TextTimeRecord("Fifth", 0, 15)}));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoMP4,
                95,
                5,
                new List<TextTimeRecord> {new TextTimeRecord("Sixth", 0, 5)}));
            var stopWatch = Stopwatch.StartNew();
            ffmpegVideoRenderer.StartRender(OutputFolder + "Cut2Episodes_WithText_NoImages.mp4");
            Console.Write(stopWatch.Elapsed);
            Assert.IsTrue(File.Exists(OutputFolder + "Cut2Episodes_WithText_NoImages.mp4"));
        }

        [Test]
        public void Cut5Episodes_SameSource_NoText_NoImages_PerformanceTest()
        {
            const string Output = OutputFolder + "Cut5Episodes_NoText_NoImages.mkv";
            var sw = Stopwatch.StartNew(); 
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 1250, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 750, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 550, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 350, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 1750, 15));
            ffmpegVideoRenderer.StartRender(Output);
            sw.Stop();
            Assert.IsTrue(File.Exists(Output));
            Assert.LessOrEqual(sw.ElapsedMilliseconds, 20000);
        }

        [Test]
        public void Cut5Episodes_SameSource_WithText_NoImages_ProgressTest()
        {
            const string Output = OutputFolder + "Cut5Episodes_NoText_NoImages.mkv";
            var sw = Stopwatch.StartNew(); 
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 1250, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 750, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 550, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 350, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 1750, 15));

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
        public void Cut5Episodes_YoutubeSource_WithText_NoImages_ProgressTest()
        {
            const string Output = OutputFolder + "Cut5Episodes_NoText_NoImages.mkv";
            var sw = Stopwatch.StartNew(); 
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealYoutubeVideoUrl, 150, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealYoutubeVideoUrl, 250, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealYoutubeVideoUrl, 350, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealYoutubeVideoUrl, 450, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealYoutubeVideoUrl, 550, 15));

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
        public void Cut3Episodes_DifferentFormat_WithText_NoImages_Test()
        {
            const string Output = OutputFolder + "Cut3Episodes_DifferentFormat_NoText_NoImages.mkv";
            var sw = Stopwatch.StartNew();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI2,
                1000,
                15,
                new List<TextTimeRecord> {new TextTimeRecord("TEXT1", 0, 15)},
                new List<DrawImageTimeRecord>()));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI,
                1250,
                15,
                new List<TextTimeRecord> {new TextTimeRecord("TEXT2", 0, 15)},
                new List<DrawImageTimeRecord>()));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoMP4,
                140,
                15,
                new List<TextTimeRecord> {new TextTimeRecord("TEXT3", 0, 15)},
                new List<DrawImageTimeRecord>()));
            ffmpegVideoRenderer.StartRender(Output, new Size(1280, 720));
            sw.Stop();
            Thread.Sleep(1000); //чтобы лог закончил заполняться.
            Assert.IsTrue(File.Exists(Output));
        }

        [Test]
        public void Cut3Episodes_DifferentFormat_NoText_NoImages_Test()
        {
            const string Output = OutputFolder + "Cut3Episodes_DifferentFormat_NoText_NoImages.mkv";
            var sw = Stopwatch.StartNew();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI2, 1000, 15, new List<TextTimeRecord>(), new List<DrawImageTimeRecord>()));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 1250, 15, new List<TextTimeRecord>(), new List<DrawImageTimeRecord>()));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoMP4, 140, 15, new List<TextTimeRecord>(), new List<DrawImageTimeRecord>()));
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
        public void Cut25Episodes_WithText_NoImages_PerformanceTest()
        {
            var sw = Stopwatch.StartNew();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            int totalDuration = 0;
            for (int i = 0; i < 25; i++)
            {
                ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(
                    SampleFiles.RealInputVideoAVI,
                    totalDuration,
                    15,
                    new List<TextTimeRecord> {new TextTimeRecord(i.ToString(), 0, 15)}));
                totalDuration += 30;
            }
            ffmpegVideoRenderer.StartRender(OutputFolder + "Cut25Episodes_WithText_NoImages.mp4");
            sw.Stop();
            //Assert.LessOrEqual(sw.Elapsed, new TimeSpan(0, 0, 20), sw.Elapsed.ToString());
            Assert.IsTrue(File.Exists(OutputFolder + "Cut25Episodes_WithText_NoImages.mp4"));
        }

        [Test]
        public void Cut25Episodes_NoText_WithImages_Test()
        {
            var sw = Stopwatch.StartNew();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            int currentStart = 0;
            for (int i = 0; i < 25; i++)
            {
                ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(
                    SampleFiles.RealInputVideoAVI2,
                    currentStart,
                    15,
                    new List<TextTimeRecord>(),
                    new List<DrawImageTimeRecord>
                    {
                        new DrawImageTimeRecord(File.ReadAllBytes(SampleFiles.SamplePngImage), 100, 100, 3, 5)
                    }));
                currentStart += 100;
            }
            ffmpegVideoRenderer.StartRender(OutputFolder + "Cut25Episodes_WithText_WithImages.mp4");
            sw.Stop();
            Assert.LessOrEqual(sw.Elapsed, new TimeSpan(0, 3, 20), sw.Elapsed.ToString());
            Assert.IsTrue(File.Exists(OutputFolder + "Cut25Episodes_WithText_WithImages.mp4"));
        }

        [Test]
        public void Cut25Episodes_WithText_WithImages_PerformanceTest()
        {
            var sw = Stopwatch.StartNew();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            int currentStart = 0;
            for (int i = 0; i < 25; i++)
            {
                ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(
                    SampleFiles.RealInputVideoAVI2,
                    currentStart,
                    15,
                    new List<TextTimeRecord> { new TextTimeRecord(i.ToString(), 0, 15) },
                    new List<DrawImageTimeRecord>
                    {
                        new DrawImageTimeRecord(File.ReadAllBytes(SampleFiles.SamplePngImage), 100, 100, 3, 5)
                    }));
                currentStart += 100;
            }
            ffmpegVideoRenderer.StartRender(OutputFolder + "Cut25Episodes_WithText_WithImages.mp4");
            sw.Stop();
            Assert.LessOrEqual(sw.Elapsed, new TimeSpan(0, 3, 20), sw.Elapsed.ToString());
            Assert.IsTrue(File.Exists(OutputFolder + "Cut25Episodes_WithText_WithImages.mp4"));
        }

        [Test]
        public void Cut25Episodes_WithText_WithImages_WithTimeWarps_PerformanceTest()
        {
            var sw = Stopwatch.StartNew();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            int totalDuration = 0;
            for (int i = 0; i < 25; i++)
            {
                ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(
                    SampleFiles.RealInputVideoAVI2,
                    totalDuration,
                    15,
                    new List<TextTimeRecord> {new TextTimeRecord(i.ToString(), 0, 15)},
                    new List<DrawImageTimeRecord>
                    {
                        new DrawImageTimeRecord(File.ReadAllBytes(SampleFiles.SamplePngImage), 100, 100, 3, 5)
                    },
                    new List<TimeWarpRecord> {new TimeWarpRecord(3, 6, 0.5)}));
                totalDuration += 20;
            }

            ffmpegVideoRenderer.StartRender(OutputFolder + "Cut25Episodes_WithText_WithImages_WithTimeWarps.mp4");
            sw.Stop();
            Assert.LessOrEqual(sw.Elapsed, new TimeSpan(0, 3, 0), sw.Elapsed.ToString());
            Assert.IsTrue(File.Exists(OutputFolder + "Cut25Episodes_WithText_WithImages_WithTimeWarps.mp4"));
        }

        [Test]
        public void Cut2SameEpisodes_WithText_NoImages_PerformanceTest()
        {
            var sw = Stopwatch.StartNew();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec,
                20,
                15,
                new List<TextTimeRecord> { new TextTimeRecord("First episode", 0, 15) }));
            ffmpegVideoRenderer.AddVideoEpisodes(
                new VideoRenderOption(SampleFiles.Helicopter_1min_48sec,
                    20,
                    15,
                    new List<TextTimeRecord> { new TextTimeRecord("Second episode", 0, 15) }));
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
            this.Cut6Episodes_WithText_NoImages_DIVX_Test();
            
            // ждем пока лог заполнится
            Thread.Sleep(1000);
            var logText = File.ReadAllText(LogFilePath);

            var regex = new Regex("\r\n-----------*.?--------------\r\n");
            Assert.AreEqual(4, regex.Matches(logText).Count);
        }

        [Test]
        public void Cut1EpisodeWithText_FinishCallback_Test()
        {
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.Helicopter_1min_48sec,
                40,
                55,
                new List<TextTimeRecord> {new TextTimeRecord("Overlay text", 0, 55)}));
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
        public void Cut3Episodes_WithText_OneImages_OneTimewarp_Progress_Test()
        {
            var sw = Stopwatch.StartNew();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            var images = new List<DrawImageTimeRecord>
            {
                new DrawImageTimeRecord(File.ReadAllBytes(SampleFiles.SamplePngImage), 100, 100, 1, 4)
            };
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI,
                20,
                15,
                new List<TextTimeRecord> {new TextTimeRecord("First text", 0, 15)},
                images));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI,
                300,
                15,
                new List<TextTimeRecord> {new TextTimeRecord("Third text", 0, 15)},
                new List<DrawImageTimeRecord>(),
                new List<TimeWarpRecord> {new TimeWarpRecord(3, 6, 0.5)}));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI,
                500,
                15,
                new List<TextTimeRecord>(),
                new List<DrawImageTimeRecord>()));
            double currentProgress = 0;
            var outputFilePath = OutputFolder + "Cut3Episodes_WithText_OneImages_OneTimewarp.mkv";
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
        public void Cut4Episodes_NoText_NoImages_Cancel_Test()
        {
            var cancelationTokenSource = new CancellationTokenSource();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer(cancelationTokenSource);
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 40, 55));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 40, 55));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 40, 55));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 40, 55));
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
        public void Cut1EpisodeWithText_FromSmallSizeVideo_Test()
        {
            var cancelationTokenSource = new CancellationTokenSource();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer(cancelationTokenSource);
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.LowQualityVideo,
                40,
                15,
                new List<TextTimeRecord> {new TextTimeRecord("Hello world. Go back and force. Return to the middle.", 0, 15)}));
            ffmpegVideoRenderer.StartRender(OutputFolder + "Cut1EpisodeWithText_FromSmallSizeVideo.mp4", new Size(640, 360));

            Assert.IsTrue(File.Exists(OutputFolder + "Cut1EpisodeWithText_FromSmallSizeVideo.mp4"));
        }

        [Test]
        public void Cut1EpisodeWithSpecialSymbolsInText_Test()
        {
            var cancelationTokenSource = new CancellationTokenSource();
            var ffmpegVideoRenderer = new FFMpegVideoRenderer(cancelationTokenSource);
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.LowQualityVideo,
                40,
                15,
                new List<TextTimeRecord> {new TextTimeRecord("Don't do it again: \"%MARGUS%\"", 0, 15)}));
            ffmpegVideoRenderer.StartRender(OutputFolder + "Cut1EpisodeWithSpecialSymbolsInText.mp4", new Size(640, 360));

            Assert.IsTrue(File.Exists(OutputFolder + "Cut1EpisodeWithSpecialSymbolsInText.mp4"));
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
                        Assert.IsTrue(exception.Message.Contains("No such file or directory"));
                    });
            Assert.IsFalse(File.Exists(OutputFolder + "Cut1Episode_NoText_NoImages.mp4"));
        }

        [Test]
        public void CutFromTheEndOfLowQualityVideo_ExceptionTest()
        {
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.LowQualityVideo, 691, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 1386, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI2, 2198, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.LowQualityVideo, 3157, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.LowQualityVideo, 3644, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.LowQualityVideo, 4251, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.LowQualityVideo, 4800, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.LowQualityVideo, 5436, 15));

            var sw = Stopwatch.StartNew();
            double currentProgress = 0;
            ffmpegVideoRenderer.StartRender(OutputFolder + "CutFromTheEndOfLowQualityVideo.mp4", new Size(640, 360),
                (fileName, percent, currentTime, estimatedTime) =>
                {
                    //currentProgress = percent;
                    //Console.WriteLine("{0}% {1}sec from {2}sec", percent, currentTime, estimatedTime);
                    //Assert.GreaterOrEqual(currentTime, 0);
                },
                (totalDuration, exception) =>
                {
                    //Assert.AreEqual(1, currentProgress);
                });
            sw.Stop();
            Assert.IsTrue(File.Exists(OutputFolder + "CutFromTheEndOfLowQualityVideo.mp4"));
        }

        [TearDown]
        public void CleanUp()
        {
            Directory.Delete(OutputFolder, true);
        }
    }
}