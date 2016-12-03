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
            var thumbnail = VideoUtils.GetFrameFromVideoAsByte("SampleVideo_1280x720_1mb.mp4", 1);
            Assert.AreEqual(7152, thumbnail.Length, 100);
        }
    }
}
