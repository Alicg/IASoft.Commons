using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Utils.Common.Extensions;

namespace YoutubeWrapper
{
    public class YoutubeFacade
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string applicationName;
        private readonly string channelId;
        private readonly SecureString savedRefreshToken;

        private UserCredential connectedUserCredential;

        public YoutubeFacade(string clientId, string clientSecret, string applicationName, string channelId, SecureString savedRefreshToken)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.applicationName = applicationName;
            this.channelId = channelId;
            this.savedRefreshToken = savedRefreshToken;
        }

        public async Task<string> UploadVideo(
            string filePath,
            string title,
            string description,
            string playlistName,
            CancellationToken cancellationToken,
            Action<double, double> progressChanged = null)
        {
            var youtubeService = this.ConnectToYoutubeService();

            var video = new Video
            {
                Snippet = new VideoSnippet {Title = title, Description = description, Tags = new[] {"Sports"}},
                Status = new VideoStatus {PrivacyStatus = "unlisted"}
            };
            string createdVideoId;
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                long bytesSent = 0;
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                if (progressChanged != null)
                {
                    videosInsertRequest.ProgressChanged +=
                        progress =>
                        {
                            bytesSent = progress.BytesSent;
                            progressChanged(videosInsertRequest.ContentStream.Length, progress.BytesSent);
                        };
                }
                const int Kb = 1024;
                const int MinimumChunkSize = 256 * Kb;
                videosInsertRequest.ChunkSize = MinimumChunkSize;
                IUploadProgress uploadProgress = null;
                try
                {
                    uploadProgress = await videosInsertRequest.UploadAsync(cancellationToken);
                }
                catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception e)
                {
                    var lastException = e;
                    int maxRetrievesCount = 3;
                    int retriesCount = 0;
                    long previousBytesSent = 0;
                    long minimalKbToProlongUpload = 500;
                    while (maxRetrievesCount > retriesCount && (uploadProgress == null || uploadProgress.Status != UploadStatus.Completed))
                    {
                        retriesCount++;
                        try
                        {
                            uploadProgress = await videosInsertRequest.ResumeAsync(cancellationToken);
                        }
                        catch (Exception exception)
                        {
                            lastException = exception;

                            // Если мы залили хотябы 500кб с предыдущего разрыва, значит обнуляем счетчик попыток. Так понемногу и всё видео зальём.
                            if (Math.Abs(bytesSent - previousBytesSent) > minimalKbToProlongUpload)
                            {
                                retriesCount = 0;
                            }
                            previousBytesSent = bytesSent;
                            Task.Delay(TimeSpan.FromSeconds(2), cancellationToken).Wait(cancellationToken);
                        }
                    }
                    if (uploadProgress == null || uploadProgress.Status != UploadStatus.Completed)
                    {
                        throw new YoutubeException($"Video upload failed (Retries count = {retriesCount}). See inner exception for details.", lastException);
                    }
                }
                createdVideoId = videosInsertRequest.ResponseBody.Id;
                if (createdVideoId == null)
                {
                    throw new YoutubeException(videosInsertRequest.ResponseBody.Status.UploadStatus);
                }
            }
            if (playlistName != null)
            {
                this.AddToPlayList(createdVideoId, playlistName, youtubeService);
            }
            return createdVideoId;
        }

        public string DeleteVideo(string videoId)
        {
            var youtubeService = this.ConnectToYoutubeService();
            var deleteRequest = youtubeService.Videos.Delete(videoId);
            return deleteRequest.Execute();
        }

        public string GetVideoInfo(string videoId)
        {
            var youtubeService = this.ConnectToYoutubeService();
            var searchRequest = youtubeService.Videos.List("snippet");
            searchRequest.Id = videoId;
            var video = searchRequest.Execute().Items.FirstOrDefault();
            return video?.Id;
        }

        private void AddToPlayList(string videoId, string playListName, YouTubeService service)
        {
            var playlistRequest = service.Playlists.List("snippet");
            playlistRequest.ChannelId = this.channelId;
            playlistRequest.MaxResults = 50;
            var allPlaylists = playlistRequest.Execute();
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
            service.PlaylistItems.Insert(newPlaylistItem, "snippet").Execute();
        }

        private YouTubeService ConnectToYoutubeService()
        {
            if (this.connectedUserCredential == null)
            {
                var authorizeTask = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets {ClientId = this.clientId, ClientSecret = this.clientSecret}, 
                    new[] {YouTubeService.Scope.YoutubeUpload, YouTubeService.Scope.Youtube},
                    Environment.UserName,
                    CancellationToken.None,
                    new SavedDataStore(this.savedRefreshToken.ConvertToUnsecureString())
                );
                authorizeTask.Wait();
                this.connectedUserCredential = authorizeTask.Result;
            }

            if (this.connectedUserCredential.Token.IsExpired(SystemClock.Default))
            {
                var refreshTokenTask = this.connectedUserCredential.RefreshTokenAsync(CancellationToken.None);
                refreshTokenTask.Wait();
                if (!refreshTokenTask.Result)
                {
                    throw new InvalidOperationException("Access token was expired and new one can't be requested due.");
                }
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                HttpClientInitializer = this.connectedUserCredential,
                GZipEnabled = true,
                ApplicationName = this.applicationName
            });

            return youtubeService;
        }
    }
}