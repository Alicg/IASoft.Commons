using System.IO;
using System.Threading;
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
                "1/QdUuZknDmquIdTo46TI8kgnhSMG9bEvAvqnWGYq4mRk".ConvertToSecureString());
            var videoId = youtubeFacade
                .UploadVideo(
                    @"W:\Home\sport\SportVideoAnalyzer\src\Shells\SVA\bin\Debug\11.02.2017 with Akimenko Andrii, Artemenko Dmytro, Konstantinov Ievgen.avi",
                    "Test video",
                    "Test description",
                    "Uploaded");
            
            Assert.IsNotNull(youtubeFacade.GetVideoInfo(videoId));

            var deleteResult = youtubeFacade.DeleteVideo(videoId);

            // ждем 2 сек пока ютуб расчехлится, что видео удалено.
            Thread.Sleep(2000);
            Assert.IsNull(youtubeFacade.GetVideoInfo(videoId));
        }
    }
}