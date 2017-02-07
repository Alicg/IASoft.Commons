using System;

namespace Video.Utils
{
    internal interface IVideoRenderer
    {
        void AddVideoEpisodes(params VideoRenderOption[] videoRenderOption);
        void StartRender(string outputFile, Action<string, double> callbackAction, Action<double, string> finishAction);
        void StartRenderAsync(string outputFile, Action<string, double> callbackAction, Action<double, string> finishAction);
        void Cancel();
    }
}