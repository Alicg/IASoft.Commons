using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace FFMpegWrapper
{
    public class FFMpegLogger
    {
        public static Logger Instance { get; } = InitLogger();

        private static Logger InitLogger()
        {
            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget
            {
                FileName = "${basedir}/ffmpeg.log",
                Layout = "${message}"
            };

            var asyncFileTarget = new AsyncTargetWrapper(fileTarget);

            config.AddTarget("file", asyncFileTarget);

            var rule2 = new LoggingRule("*", LogLevel.Debug, asyncFileTarget);
            config.LoggingRules.Add(rule2);
            
            LogManager.Configuration = config;

            return LogManager.GetCurrentClassLogger();
        }
    }
}