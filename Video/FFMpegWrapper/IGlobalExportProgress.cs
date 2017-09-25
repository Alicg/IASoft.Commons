namespace FFMpegWrapper
{
    public interface IGlobalExportProgress
    {
        void SetCurrentOperationProgress(double currentSeconds, double totalEstimatedSeconds);

        void IncreaseOperationsDone();
    }
}