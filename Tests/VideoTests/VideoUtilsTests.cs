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
    }
}
