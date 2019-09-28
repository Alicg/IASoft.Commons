using System;
using System.Drawing.Printing;

namespace Video.Utils
{
    public class WebVideoInfo
    {
        public string VideoTitle { get; set; }
        
        public string VideoDescription { get; set; }
        
        public string OriginalUrl { get; set; }

        public string ThumbnailUrl { get; set; }
        
        public string VideoStreamUrl { get; set; }
        
        public string AudioStreamUrl { get; set; }
     
        public static implicit operator String(WebVideoInfo info)
        {
            return info.VideoStreamUrl;
        }
    }
}