using System;
using System.Text.RegularExpressions;

namespace FFMpegWrapper
{
    public class VideoInfoParser
    {
        public FFMpegVideoInfo ParseFFMpegInfo(string ffmpegLog)
        {
            var regex = new Regex(@"Duration: (?<DurationH>.*?):(?<DurationM>.*?):(?<DurationS>.*?)\.(?<DurationMS>.*?),[^~]*?Video:[^~]*?(?<Width>\d{3,5})x(?<Height>\d{3,5})");
            var match = regex.Match(ffmpegLog);
            var width = Convert.ToInt32(match.Groups["Width"].Value);
            var height = Convert.ToInt32(match.Groups["Height"].Value);
            var durationHour = Convert.ToInt32(match.Groups["DurationH"].Value);
            var durationMin = Convert.ToInt32(match.Groups["DurationM"].Value);
            var durationSec = Convert.ToInt32(match.Groups["DurationS"].Value);
            var durationMSec = Convert.ToInt32(match.Groups["DurationMS"].Value);
            var videoInfo = new FFMpegVideoInfo(width, height, new TimeSpan(0, durationHour, durationMin, durationSec, durationMSec));
            return videoInfo;
        }
    }
}