using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace Video.Utils
{
    internal interface IVideoRenderer
    {
        void AddVideoEpisodes(params VideoRenderOption[] videoRenderOption);
        void StartRender(string outputFile, Size outputSize, ProcessPriorityClass processPriorityClass, Action<string, double, double, double> callbackAction, Action<double, Exception> finishAction);
        Task StartRenderAsync(string outputFile, Size outputSize, ProcessPriorityClass processPriorityClass, Action<string, double, double, double> callbackAction, Action<double, Exception> finishAction);
    }
}