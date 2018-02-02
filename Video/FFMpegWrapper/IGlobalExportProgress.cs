using System.Diagnostics;

namespace FFMpegWrapper
{
    public interface IGlobalExportProgress
    {
        void SetCurrentOperationProgress(double currentSeconds, double totalEstimatedSeconds, int processId);

        void IncreaseOperationsDone(int processId, int count = 1);
    }
}