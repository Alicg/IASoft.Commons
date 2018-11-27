using System.IO;
using System.Windows.Forms;
using Application = System.Windows.Application;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace IASoft.WPFCommons
{
    public class IoDialogsService : IIoDialogsService
    {
        /// <param name="initialDirectory"></param>
        /// <param name="defaultExtension">example: .avi</param>
        /// <param name="filter">example: Video files (.avi;.vob;.mpg;.wmv;.mts;.mp4;.ts;)|*.avi;*.vob;*.mpg;*.wmv;*.mts;*.mp4;*.ts;</param>
        /// <param name="title">Default value is <see cref="string.Empty"/>/></param>
        /// <returns></returns>
        public string OpenFile(string initialDirectory, string defaultExtension, string filter, string title = null)
        {
            title = title ?? string.Empty;
            var dlg = new OpenFileDialog
            {
                InitialDirectory = initialDirectory,
                DefaultExt = defaultExtension,
                Filter = filter,
                Title = title
            };
            var result = dlg.ShowDialog(Application.Current.MainWindow);
            if (result == true)
            {
                return dlg.FileName;
            }
            return null;
        }

        public string OpenFolder(string initialDirectory)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    return dialog.SelectedPath;
                }
            }
            return null;
        }

        public string SaveFile(string initialDirectory, string suggestedFileName, string outputFormatFilter, out bool cancelled, string title = null)
        {
            title = title ?? string.Empty;
            if (!Directory.Exists(initialDirectory))
            {
                Directory.CreateDirectory(initialDirectory);
            }

            using (var dialog = new SaveFileDialog
                {
                    InitialDirectory = initialDirectory, 
                    FileName = suggestedFileName, 
                    OverwritePrompt = true, 
                    Filter = outputFormatFilter, 
                    FilterIndex = 0,
                    Title = title,
                })
            {
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    cancelled = false;
                    return Path.Combine(dialog.InitialDirectory, dialog.FileName);
                }
            }

            cancelled = true;
            return Path.Combine(initialDirectory, suggestedFileName);
        }
    }
}
