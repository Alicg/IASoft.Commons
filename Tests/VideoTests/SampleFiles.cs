using System;
using System.IO;

namespace VideoTests
{
    public class SampleFiles
    {
        public const string Helicopter_1min_48sec = "Helicopter_DivXHT_ASP_1-48.divx";
        public const string SampleVideo_5sec = "SampleVideo_1280x720_1mb_5s.mp4";
        public const string SamplePngImage = "leo_console.png";
        
        public static readonly string InputFolder = Path.Combine(Environment.GetEnvironmentVariable("SportFolder"), "Handball videos");

        public static readonly string RealInputVideoAVI = Path.Combine(InputFolder, "Гандбол. ЛЧ. 2014-2015. 6-й тур. Багга. 25.11.2014.avi");

        public static readonly string RealInputVideoAVI2 = Path.Combine(InputFolder, "Гандбол. Ч.Е. Финал. Дания-Франция. Багга. 26.01.2014.avi");

        public static readonly string RealInputVideoMP4 = Path.Combine(InputFolder, "HDV_1626.mp4");
    }
}