using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using NUnit.Framework;
using NYoutubeDL;
using Video.Utils;

namespace VideoTests
{
    [TestFixture]
    public class FFMpegYoutubeTests
    {
        private string YoutubeVideoStreamUrl;
        private string YoutubeAudioStreamUrl;
        private const string OutputFolder = "FFMpegYoutubeTests//";
        private const string YoutubeVideoUrl = "https://www.youtube.com/watch?v=rxK0pkl8tbE";

        public FFMpegYoutubeTests()
        {
            var videoUtils = new VideoUtils();
            var webVideoInfo = videoUtils.GetWebVideoInfo(YoutubeVideoUrl);
            this.YoutubeVideoStreamUrl = webVideoInfo.VideoStreamUrl;
            this.YoutubeAudioStreamUrl = webVideoInfo.AudioStreamUrl;

            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }
        }
        
        [Test]
        public void VideoInfo_Test()
        {
            var videoUtils = new VideoUtils();
            var videoInfo = videoUtils.GetVideoInfo(this.YoutubeVideoStreamUrl);
            Assert.AreEqual(1080, videoInfo.Height);
            Assert.AreEqual(1920, videoInfo.Width);
        }

        [Test]
        public void YoutubeDL_Test()
        {
            var youtubeDl = new YoutubeDL(SampleFiles.RealYoutubeVideoUrl);
            Assert.IsNotNull(youtubeDl.Info.VideoSize);
        }

        [Test]
        public void Cut5Episodes_SameYoutubeSource_WithText_NoImages_ProgressTest()
        {
            const string Output = OutputFolder + "Cut5Episodes_SameYoutubeSource_WithText_NoImages_ProgressTest.mkv";
            var sw = Stopwatch.StartNew(); 
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(this.YoutubeVideoStreamUrl, this.YoutubeAudioStreamUrl, 1250, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(this.YoutubeVideoStreamUrl, this.YoutubeAudioStreamUrl, 750, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(this.YoutubeVideoStreamUrl, this.YoutubeAudioStreamUrl, 550, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(this.YoutubeVideoStreamUrl, this.YoutubeAudioStreamUrl, 350, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(this.YoutubeVideoStreamUrl, this.YoutubeAudioStreamUrl, 1750, 15));

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
        public void Cut5Episodes_YoutubeAndDiskSource_WithText_NoImages_ProgressTest()
        {
            const string Output = OutputFolder + "Cut5Episodes_SameYoutubeSource_WithText_NoImages_ProgressTest.mkv";
            var sw = Stopwatch.StartNew(); 
            var ffmpegVideoRenderer = new FFMpegVideoRenderer();
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 1250, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(this.YoutubeVideoStreamUrl, this.YoutubeAudioStreamUrl, 750, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 550, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(this.YoutubeVideoStreamUrl, this.YoutubeAudioStreamUrl, 350, 15));
            ffmpegVideoRenderer.AddVideoEpisodes(new VideoRenderOption(SampleFiles.RealInputVideoAVI, 1750, 15));

            double previousProgress = 0;
            ffmpegVideoRenderer.StartRender(
                Output,
                new Size(1280, 720),
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
    }
}