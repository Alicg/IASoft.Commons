using System;
using System.Threading.Tasks;

namespace Video.Utils
{
    internal interface IVideoRenderer
    {
        void AddVideoEpisodes(params VideoRenderOption[] videoRenderOption);
        void StartRender(string outputFile, Action<string, double> callbackAction, Action<double, Exception> finishAction);
        Task StartRenderAsync(string outputFile, Action<string, double> callbackAction, Action<double, Exception> finishAction);
    }
}