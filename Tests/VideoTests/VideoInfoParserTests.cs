using FFMpegWrapper;
using NUnit.Framework;

namespace VideoTests
{
    [TestFixture]
    public class VideoInfoParserTests
    {
        [Test]
        public void ParseTest()
        {
            const string TestFFMpegLog = @"Input #0, avi, from 'out.avi':
  Metadata:
    encoder         : Lavf57.82.101
  Duration: 03:20:21.46, start: 0.000000, bitrate: 1868 kb/s
    Stream #0:0: Video: mpeg4 (Advanced Simple Profile) (DX50 / 0x30355844), yuv420p, 704x400 [SAR 1:1 DAR 44:25], 1865 kb/s, 25 fps, 25 tbr, 25 tbn, 25 tbc";
            var parser = new VideoInfoParser();
            var info = parser.ParseFFMpegInfo(TestFFMpegLog);
            Assert.AreEqual(704, info.Width);
            Assert.AreEqual(400, info.Height);
            Assert.AreEqual(3, info.Duration.Hours);
            Assert.AreEqual(20, info.Duration.Minutes);
            Assert.AreEqual(21, info.Duration.Seconds);
            Assert.AreEqual(46, info.Duration.Milliseconds);
        }
    }
}