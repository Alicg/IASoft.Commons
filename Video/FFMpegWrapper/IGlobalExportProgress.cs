namespace FFMpegWrapper
{
    public interface IGlobalExportProgress
    {
        void SetCurrentOperationProgress(double currentSeconds, double totalEstimatedSeconds, int processId);

        void IncreaseOperationsDone(int processId);
    }
}