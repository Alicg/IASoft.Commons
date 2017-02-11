namespace FFMpegWrapper
{
    public interface IGlobalExportProgress
    {
        void SetCurrentOperationProgress(double currentProgress);

        void IncreaseOperationsDone();
    }
}