using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Utils.Common.Extensions;
using YoutubeWrapper;

namespace VideoTests
{
    [TestFixture]
    public class YoutubeTests
    {
        private const string ClientId = "746039047098-l4nsbmuc8qcb71itol98n3vdeshg2jsr.apps.googleusercontent.com";
        private const string ClientSecret = "C7JJLJD1ERkfhVc9dzTMs0qU";

        [Test]
        public void RetriesUploads_Test()
        {
            var youtubeFacade = new YoutubeFacade(
                ClientId,
                ClientSecret,
                "Sport Video Analyzer",
                "UCHl2h1aE8so4oV8tqQRYQDQ",
                "1/QdUuZknDmquIdTo46TI8kgnhSMG9bEvAvqnWGYq4mRk".ConvertToSecureString());
            var videoUploadTask = youtubeFacade
                .UploadVideo(
                    @"W:\Home\sport\SportVideoAnalyzer\src\Shells\SVA\bin\Debug\16.02.2017 with no players.avi",
                    "Test video",
                    "Test description",
                    "Uploaded",
                    CancellationToken.None);

            videoUploadTask.Wait();
            var videoId = videoUploadTask.Result;
            
            Assert.IsNotNull(youtubeFacade.GetVideoInfo(videoId));

            var deleteResult = youtubeFacade.DeleteVideo(videoId);

            // ждем 2 сек пока ютуб расчехлится, что видео удалено.
            Thread.Sleep(2000);
            Assert.IsNull(youtubeFacade.GetVideoInfo(videoId));
        }

        [Test]
        public void UploadCancel_Test()
        {
            var youtubeFacade = new YoutubeFacade(
                ClientId,
                ClientSecret,
                "Sport Video Analyzer",
                "UCHl2h1aE8so4oV8tqQRYQDQ",
                "1/QdUuZknDmquIdTo46TI8kgnhSMG9bEvAvqnWGYq4mRk".ConvertToSecureString());
            var cts = new CancellationTokenSource();
            var videoUploadTask = youtubeFacade
                .UploadVideo(
                    @"W:\Home\sport\SportVideoAnalyzer\src\Shells\SVA\bin\Debug\16.02.2017 with no players.avi",
                    "Test video",
                    "Test description",
                    "Uploaded",
                    cts.Token);
            cts.CancelAfter(TimeSpan.FromSeconds(3));
            try
            {
                videoUploadTask.Wait();
            }
            catch (AggregateException e)
            {
                Assert.IsTrue(e.InnerExceptions[0] is TaskCanceledException);
                Assert.Pass();
            }
            Assert.Fail("TaskCanceledException was expected.");
        }
    }
}