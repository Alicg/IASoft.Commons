using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Utils.Common.Extensions;

namespace YoutubeWrapper
{
    public class YoutubeFacade
    {
        private readonly Stream secretStream;
        private readonly string applicationName;
        private readonly string channelId;
        private readonly SecureString savedRefreshToken;

        private UserCredential connectedUserCredential;

        public YoutubeFacade(Stream secretStream, string applicationName, string channelId, SecureString savedRefreshToken)
        {
            this.secretStream = secretStream;
            this.applicationName = applicationName;
            this.channelId = channelId;
            this.savedRefreshToken = savedRefreshToken;
        }

        public async Task<string> UploadVideo(string filePath, string title, string description, string playlistName, Action<double, double> progressChanged = null)
        {
            var youtubeService = await this.ConnectToYoutubeService();

            var video = new Video
            {
                Snippet = new VideoSnippet {Title = title, Description = description, Tags = new[] {"Sports"}},
                Status = new VideoStatus {PrivacyStatus = "unlisted"}
            };
            string createdVideoId;
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                if (progressChanged != null)
                {
                    videosInsertRequest.ProgressChanged +=
                        progress => progressChanged(videosInsertRequest.ContentStream.Length, progress.BytesSent);
                }
                const int Kb = 1024;
                const int MinimumChunkSize = 256 * Kb;
                videosInsertRequest.ChunkSize = MinimumChunkSize;
                await videosInsertRequest.UploadAsync();
                createdVideoId = videosInsertRequest.ResponseBody.Id;
            }
            if (createdVideoId == null)
            {
                return "Upload failed";
            }
            if (playlistName != null)
            {
                this.AddToPlayList(createdVideoId, playlistName, youtubeService);
            }
            return $"https://www.youtube.com/watch?v={createdVideoId}";
        }

        private async void AddToPlayList(string videoId, string playListName, YouTubeService service)
        {
            var playlistRequest = service.Playlists.List("snippet");
            playlistRequest.ChannelId = this.channelId;
            var allPlaylists = await playlistRequest.ExecuteAsync();
            var playlist = allPlaylists.Items.FirstOrDefault(v => v.Snippet.Title == playListName);
            if (playlist == null)
            {
                return;
            }
            var newPlaylistItem = new PlaylistItem
            {
                Snippet =
                    new PlaylistItemSnippet
                    {
                        PlaylistId = playlist.Id,
                        ResourceId = new ResourceId {Kind = "youtube#video", VideoId = videoId}
                    }
            };
            await service.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();
        }

        private async Task<YouTubeService> ConnectToYoutubeService()
        {
            if (this.connectedUserCredential == null)
            {
                this.connectedUserCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(this.secretStream).Secrets,
                    new[] {YouTubeService.Scope.YoutubeUpload},
                    Environment.UserName,
                    CancellationToken.None,
                    new SavedDataStore(this.savedRefreshToken.ConvertToUnsecureString())
                );
            }

            if (this.connectedUserCredential.Token.IsExpired(SystemClock.Default))
            {
                if (!await this.connectedUserCredential.RefreshTokenAsync(CancellationToken.None))
                {
                    throw new InvalidOperationException("Access token was expired and new one can't be requested due.");
                }
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                HttpClientInitializer = this.connectedUserCredential,
                ApplicationName = this.applicationName
            });

            return youtubeService;
        }
    }
}