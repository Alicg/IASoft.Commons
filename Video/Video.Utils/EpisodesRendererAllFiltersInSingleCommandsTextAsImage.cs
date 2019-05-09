using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Threading;
using FFMpegWrapper;
using Utils;
using Utils.Extensions;

namespace Video.Utils
{
    public class EpisodesRendererAllFiltersInSingleCommandsTextAsImage : EpisodesRendererAllFiltersInSingleCommands
    {
        public EpisodesRendererAllFiltersInSingleCommandsTextAsImage(IList<VideoRenderOption> videoRenderOptions, string outputFile, Size outputSize, ProcessPriorityClass rendererProcessPriorityClass, IGlobalExportProgress globalExportProgress, CancellationToken cancellationToken) : base(videoRenderOptions, outputFile, outputSize, rendererProcessPriorityClass, globalExportProgress, cancellationToken)
        {
        }

        protected override void CutAndConcatAndRenderTextAndImageAndTimeWarps(List<FFMpegCutInfo> cutInfos, FFMpeg ffMpeg, TemporaryFilesStorage temporaryFilesStorage)
        {
            foreach (var videoRenderOption in this.VideoRenderOptions)
            {
                if (videoRenderOption.OverlayTextTimeTable != null && videoRenderOption.OverlayTextTimeTable.Any())
                {
                    var textImages = videoRenderOption.OverlayTextTimeTable.Select(
                        v =>
                        {
                            const int FontSizeFor1024Width = 30;
                            var fontSize = this.OutputSize.IsEmpty ? FontSizeFor1024Width : ((double)this.OutputSize.Width / 1024) * FontSizeFor1024Width;
                            var image = StringUtils.DrawTextOnImage(v.Lines.Aggregate((t, c) => t + c), Color.Transparent, Color.DarkGreen, new Font(SystemFonts.DefaultFont.FontFamily, (int)fontSize), this.OutputSize.Width - 10);
                            var left = this.OutputSize.Width / 2 - image.Width / 2;
                            var top = this.OutputSize.Height - image.Height - 15;
                            return new DrawImageTimeRecord(image.ToBytes(ImageFormat.Bmp), left, top, v.StartSecond, v.EndSecond);
                        });
                    videoRenderOption.ImagesTimeTable.AddRange(textImages);
                    videoRenderOption.OverlayTextTimeTable.Clear();
                }
            }

            base.CutAndConcatAndRenderTextAndImageAndTimeWarps(cutInfos, ffMpeg, temporaryFilesStorage);
        }
    }
}