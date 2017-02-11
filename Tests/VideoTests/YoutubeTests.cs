using System.IO;
using System.Security;
using NUnit.Framework;
using Utils.Common.Extensions;
using YoutubeWrapper;

namespace VideoTests
{
    [TestFixture]
    public class YoutubeTests
    {
        private readonly Stream secretStream;

        public YoutubeTests()
        {
            this.secretStream = typeof(YoutubeTests).Assembly.GetManifestResourceStream("VideoTests.client_id.json");
        }

        [Test]
        public void RetriesUploads_Test()
        {
            var youtubeFacade = new YoutubeFacade(
                this.secretStream,
                "Sport Video Analyzer",
                "UCHl2h1aE8so4oV8tqQRYQDQ",
                "1/kBHG-lOSjMKmNApAppfph6rE5yFGBepG7Qe6xUfMn9o".ConvertToSecureString());
            youtubeFacade.UploadVideo(SampleFiles.SampleVideo_5sec, "Test video", "Test description", "Uploaded").Wait();
        }
    }
}