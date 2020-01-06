using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using FFMpegWrapper;
using NUnit.Framework;
using Utils;
using Utils.Extensions;
using Video.Utils;

namespace VideoTests
{
    [TestFixture]
    public class VideoUtilsTests
    {
        [Test]
        public void CreateThumbnailFromSmallVideo_Test()
        {
            var thumbnail = new VideoUtils().GetFrameFromVideoAsByte(SampleFiles.SampleVideo_5sec, 1);
            Assert.AreEqual(6752, thumbnail.Length, 100);
        }

        [Test]
        public void CreateThumbnailFromSmallVideo_ParallelTest()
        {
            FFMpeg.DebugModeEnabled = true;
            var allThumbnails = new ConcurrentBag<byte[]>();
            var videoUtils = new VideoUtils();
            for (int i = 0; i < 100; i++)
            {
                this.ExtractThumbnailToBag(videoUtils, allThumbnails, i % 100);
            }
            while (allThumbnails.Count != 100)
            {
                Thread.Sleep(300);
            }
            Assert.AreEqual(100, allThumbnails.Count);
        }

        [Test]
        public void ExtractThumbnailToBagFromInternet()
        {
            var videoUtils = new VideoUtils();
            var thumbnailTask = videoUtils.GetFrameFromVideoAsByteAsync("https://www.youtube.com/watch?v=Tt5Y78DioZM", 2000);
            thumbnailTask.Wait();
            var thumbnail = thumbnailTask.Result;
            File.WriteAllBytes("Thumbnail.bmp", thumbnail);
            Process.Start("Thumbnail.bmp").WaitForExit();
        }

        private async void ExtractThumbnailToBag(VideoUtils videoUtils, ConcurrentBag<byte[]> bag, int from)
        {
            var thumbnail = await videoUtils.GetFrameFromVideoAsByteAsync(SampleFiles.Helicopter_1min_48sec, from);
            bag.Add(thumbnail);
        }

        [Test]
        public void GetVideoInfo_Test()
        {
            var videoInfo = new VideoUtils().GetVideoInfo(SampleFiles.SampleVideo_5sec);
            Assert.AreEqual(720, videoInfo.Height);
            Assert.AreEqual(1280, videoInfo.Width);
        }

        [Test(Description = "Needs administrator rights to create symbolic link.")]
        public async void SymbolicLinkAsFFMpegInput_Test()
        {
            var symbolicLink = await FileSystemExtension.CreateFileSymbolicLink(@"D:\Dima\Work\Home\Handball videos\dragūnakturungtynės.mp4");
            var videoInfo = new VideoUtils().GetVideoInfo(symbolicLink);
            Assert.AreEqual(360, videoInfo.Height);
            Assert.AreEqual(640, videoInfo.Width);
        }

        [Test]
        public void DrawTextToImage_Test()
        {
            var img = StringUtils.DrawTextOnImage("Игра 1. Незаметные игроки выигрывают угадайки.", Color.White, Color.Red, new Font("arial", 37), 600);
            img.Save("tmp.bmp", ImageFormat.Bmp);
        }
    }
}
