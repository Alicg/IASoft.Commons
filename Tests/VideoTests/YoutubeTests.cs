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
        private const string SavedRefreshTokenToMyChannel = "1/QdUuZknDmquIdTo46TI8kgnhSMG9bEvAvqnWGYq4mRk";

        [Test]
        public void RetriesUploads_Test()
        {
            var youtubeFacade = new YoutubeFacade(
                ClientId,
                ClientSecret,
                "Sport Video Analyzer",
                SavedRefreshTokenToMyChannel.ConvertToSecureString());
            var videoUploadTask = youtubeFacade
                .UploadVideo(
                    SampleFiles.Helicopter_1min_48sec,
                    "Test video",
                    "Test description",
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
        public async void UploadToPlaylist_Test()
        {
            var youtubeFacade = new YoutubeFacade(
                ClientId,
                ClientSecret,
                "Sport Video Analyzer",
                SavedRefreshTokenToMyChannel.ConvertToSecureString());
            var videoId = await youtubeFacade
                .UploadVideo(
                    SampleFiles.Helicopter_1min_48sec,
                    "Test video",
                    "Test description",
                    CancellationToken.None);
            await youtubeFacade.AddToPlayList(videoId, "PLC66zCbeSJSxdwzMkVZMqx7zVpb4cUZz7");
            
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
                "1/QdUuZknDmquIdTo46TI8kgnhSMG9bEvAvqnWGYq4mRk".ConvertToSecureString());
            var cts = new CancellationTokenSource();
            var videoUploadTask = youtubeFacade
                .UploadVideo(
                    SampleFiles.Helicopter_1min_48sec,
                    "Test video",
                    "Test description",
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

        [Test]
        public async void GetUserChannels_Test()
        {
            var youtube = new YoutubeFacade(ClientId,
                ClientSecret,
                "IA.Episodes",
                SavedRefreshTokenToMyChannel.ConvertToSecureString());
            var channels = await youtube.GetUserChannels();
            Assert.IsTrue(channels.Any(v => v.Title == "SportVideoAnalyzer"));
        }

        [Test]
        public async void GetPlaylists_Test()
        {
            var youtube = new YoutubeFacade(ClientId,
                ClientSecret,
                "IA.Episodes",
                SavedRefreshTokenToMyChannel.ConvertToSecureString());
            var channels = await youtube.GetUserChannels();
            var channelId = channels.First(v => v.Title == "SportVideoAnalyzer").Id;
            var playlists = await youtube.GetChannelPlaylists(channelId);
            Assert.IsTrue(playlists.Count > 5);
        }

        [Test]
        [Ignore("This test requires user's input and should be invoked manually.")]
        public async Task GetRefreshToken_ManualTest()
        {
            var token = await GoogleUtils.GetRefreshToken(ClientId, ClientSecret, "IA.Episodes", GoogleScopes.Youtube);
            Assert.IsNotNullOrEmpty(token);
        }

        [Test]
        public void GetVideoInfo_Test()
        {
            var youtube = new YoutubeFacade(ClientId, ClientSecret, "IA.Episodes", SavedRefreshTokenToMyChannel.ConvertToSecureString());
            var info = youtube.GetVideoInfo("KmSnmcuUZYs");
        }
    }
}