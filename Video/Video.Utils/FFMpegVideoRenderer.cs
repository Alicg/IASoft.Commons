using Utils.Extensions;

namespace Video.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class FFMpegVideoRenderer : IVideoRenderer
    {
        private readonly ICollection<VideoRenderOption> _innerCollection = new Collection<VideoRenderOption>();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public FFMpegVideoRenderer(int exportGroupId)
        {
            ExportGroupId = exportGroupId;
        }

        public void Cancel()
        {
            if(_cancellationTokenSource != null)
                _cancellationTokenSource.Cancel();
        }

        public int ExportGroupId { get; private set; }

        public void AddVideoEpisodes(params VideoRenderOption[] videoRenderOption)
        {
            foreach (var renderOption in videoRenderOption)
            {
                _innerCollection.Add(renderOption);
            }
        }

        public void StartRender(string outputFile, string outputExt, Action<string, double> callbackAction, Action<double, string> finishAction)
        {
            Task.Run(() =>
            {
                var dt = DateTime.Now;
                var mh = new MediaHandler(VideoUtils.FfmpegPath, VideoUtils.FontsPath);
                var intermediateCollection = new Collection<string>();

                try
                {
                    var predictedTime = 0;

                    if (File.Exists(outputFile))
                        File.Delete(outputFile);

                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                        _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var totalProgress = (double)(_innerCollection.Count);

                    foreach (var renderOption in _innerCollection)
                    {
                        if (_cancellationTokenSource.Token.IsCancellationRequested)
                            _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        var intermediateFile = string.Format("{0}{1}", Guid.NewGuid(), outputExt);
                        var time = predictedTime;
                        mh.CatAndDrawTextAndDrawImage(renderOption.FilePath, renderOption.Start,
                            renderOption.End - renderOption.Start, intermediateFile, renderOption.OverlayText, renderOption.ImagesTimeTable,
                            p => callbackAction(outputFile, (time + 1*p)/totalProgress));
                        intermediateCollection.Add(intermediateFile);
                        predictedTime++;
                        //callbackAction(outputFile, ++predictedTime / totalProgress);
                    }

                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                        _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    mh.Concat(outputFile, intermediateCollection.ToArray());

                    finishAction((DateTime.Now - dt).TotalMilliseconds, null);
                }
                catch (Exception ex)
                {
                    finishAction((DateTime.Now - dt).TotalMilliseconds, ex.GetFullMessage());
                }
                finally
                {
                    foreach (var intermediateFile in intermediateCollection)
                    {
                        File.Delete(intermediateFile);
                    }
                }
            }, _cancellationTokenSource.Token);
        }

        public void ClearEpisodes()
        {
            throw new NotImplementedException();
        }
    }
}
