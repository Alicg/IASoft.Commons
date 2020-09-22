namespace IASoft.WPFCommons
{
    public interface IIoDialogsService
    {
        string OpenFile(string initialDirectory, string defaultExtension, string filter, string title = null);

        string OpenFile(string initialDirectory, string defaultExtension, string filter, out bool cancelled,
            string title = null);

        string OpenFolder(string initialDirectory);

        string SaveFile(string initialDirectory, string suggestedFileName, string outputFormatFilter, out bool cancelled, string title = null);
    }
}