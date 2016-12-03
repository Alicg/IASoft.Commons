namespace Video.Utils
{
    using System;

    public interface IVideoRenderer
    {
        void AddVideoEpisodes(params VideoRenderOption[] videoRenderOption);
        void StartRender(string outputFile, string outputExt, Action<string, double> callbackAction, Action<double, string> finishAction);
        void ClearEpisodes();
        void Cancel();
        int ExportGroupId { get; }
    }
}
