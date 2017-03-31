using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using FFMpegWrapper;
using NUnit.Framework;
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
            Assert.AreEqual(7152, thumbnail.Length, 100);
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
    }
}
