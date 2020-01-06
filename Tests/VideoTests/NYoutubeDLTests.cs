using System.Threading;
using NUnit.Framework;
using NYoutubeDL;
using NYoutubeDL.Helpers;
using NYoutubeDL.Options;

namespace VideoTests
{
    [TestFixture]
    public class NYoutubeDLTests
    {
        [Test]
        public void GetUrlTest()
        {
            var youtubeDL = new YoutubeDL();
            youtubeDL.Options.PostProcessingOptions.FfmpegLocation = @"W:\Home\sport\SportVideoAnalyzer\external\CommonSources\Video\FFMpegExecutable\ffmpeg.exe";
            youtubeDL.Options.VideoFormatOptions.Format = Enums.VideoFormat.best;
            var videoInfoTask = youtubeDL.GetDownloadInfoAsync("https://www.youtube.com/watch?v=lGOeksQe4DE");
            videoInfoTask.Wait();
            var videoInfo = videoInfoTask.Result;
            
            Assert.IsNotNull(videoInfo);
            
            youtubeDL = new YoutubeDL();
            youtubeDL.Options.PostProcessingOptions.FfmpegLocation = @"W:\Home\sport\SportVideoAnalyzer\external\CommonSources\Video\FFMpegExecutable\ffmpeg.exe";
            youtubeDL.Options.VideoFormatOptions.Format = Enums.VideoFormat.best;
            videoInfoTask = youtubeDL.GetDownloadInfoAsync("https://www.youtube.com/watch?v=lGOeksQe4DE");
            videoInfoTask.Wait();
            videoInfo = videoInfoTask.Result;
            
            Assert.IsNotNull(videoInfo);
        }
    }
}