using System;
using System.Collections.Generic;
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
        private readonly SecureString savedRefreshToken;

        private UserCredential connectedUserCredential;

        public YoutubeFacade(string clientId, string clientSecret, string applicationName, SecureString savedRefreshToken)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.applicationName = applicationName;
            this.savedRefreshToken = savedRefreshToken;
        }

        public async Task<string> UploadVideo(string filePath,
            string title,
            string description,
            CancellationToken cancellationToken,
            Action<double, double> progressChanged = null)
        {
            var youtubeService = this.ConnectToYoutubeService();

            var video = new Video
            {
                Snippet = new VideoSnippet {Title = title, Description = description, Tags = new[] {"Sports"}, DefaultLanguage = "en"},
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
                    if (uploadProgress.Exception != null)
                    {
                        throw uploadProgress.Exception;
                    }
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

        public async Task<IList<YoutubeChannel>> GetUserChannels()
        {
            var youtubeService = this.ConnectToYoutubeService();
            var channelsListRequest = youtubeService.Channels.List("snippet");
            channelsListRequest.Mine = true;
            var channelsResponse = await channelsListRequest.ExecuteAsync();
            var channels = channelsResponse.Items;
            return channels.Select(v => new YoutubeChannel{Id = v.Id, Title = v.Snippet.Title}).ToList();
        }

        public async Task<IList<YoutubePlaylist>> GetChannelPlaylists(string channelId)
        {
            var youtubeService = this.ConnectToYoutubeService();
            var playlistListRequest = youtubeService.Playlists.List("snippet");
            playlistListRequest.ChannelId = channelId;
            playlistListRequest.MaxResults = 50;
            var channelsResponse = await playlistListRequest.ExecuteAsync();
            var channels = channelsResponse.Items;
            return channels.Select(v => new YoutubePlaylist {Id = v.Id, Title = v.Snippet.Title}).ToList();
        }

        public async Task AddToPlayList(string videoId, string playListId)
        {
            var youtubeService = this.ConnectToYoutubeService();
            var newPlaylistItem = new PlaylistItem
            {
                Snippet =
                    new PlaylistItemSnippet
                    {
                        PlaylistId = playListId,
                        ResourceId = new ResourceId {Kind = "youtube#video", VideoId = videoId}
                    }
            };
            await youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();
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