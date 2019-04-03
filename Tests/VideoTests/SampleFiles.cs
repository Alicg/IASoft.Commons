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

        public static readonly string RealInputVideoAVI = Path.Combine(InputFolder, "GBL.avi");

        public static readonly string RealInputVideoAVI2 = Path.Combine(InputFolder, "Гандбол. Ч.Е. Финал. Дания-Франция. Багга. 26.01.2014.avi");

        public static readonly string RealInputVideoMP4 = Path.Combine(InputFolder, "HDV_1626.mp4");

        public static readonly string LowQualityVideo = Path.Combine(InputFolder, @"dragūnakturungtynės.mp4");

        public static readonly string RealYoutubeVideoUrl =
            "https://r3---sn-xoxgbvuxa-cuns.googlevideo.com/videoplayback?fvip=3&itag=303&expire=1522247869&clen=182141578&gir=yes&key=yt6&signature=0FA31BF4924C481426662E20C1C859C6E8D334F3.66C2DC37D4BA22FD95911AD17EC18D890A434AC3&source=youtube&dur=582.416&aitags=133%2C134%2C135%2C136%2C137%2C160%2C242%2C243%2C244%2C247%2C248%2C278%2C298%2C299%2C302%2C303&lmt=1521329535779769&requiressl=yes&initcwndbps=1336250&ipbits=0&c=WEB&sparams=aitags%2Cclen%2Cdur%2Cei%2Cgir%2Cid%2Cinitcwndbps%2Cip%2Cipbits%2Citag%2Ckeepalive%2Clmt%2Cmime%2Cmm%2Cmn%2Cms%2Cmv%2Cpl%2Crequiressl%2Csource%2Cexpire&mime=video%2Fwebm&keepalive=yes&mm=31%2C29&mn=sn-xoxgbvuxa-cuns%2Csn-2gb7sn7k&id=o-AC0Oyd_yo2BMVFiF1nlr-u8obvZs77hub5KAsqoQpWPI&mt=1522226154&mv=m&ei=XVS7WrayH5OD8gOyvYyYCQ&ms=au%2Crdu&ip=194.160.11.170&pl=16&ratebypass=yes";
    }
}