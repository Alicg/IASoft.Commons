using System;
using System.IO;
using System.Reflection;
using Utils.Extensions;

namespace Utils
{
    /// <summary>
    /// Create FileStream with supporting long file pathes (> 260 characters in path)
    /// </summary>
    public static class FileStreamCreator
    {
        public static FileStream Create(string path, FileMode mode, FileAccess access,
                                        FileShare share = FileShare.Read, int bufferSize = 4096,
                                        FileOptions options = FileOptions.None)
        {
            return Activator.CreateInstance(typeof (FileStream),
                                            BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
                                            null,
                                            new object[]
                                                {
                                                    path,
                                                    mode,
                                                    access,
                                                    share,
                                                    bufferSize,
                                                    options,
                                                    path.ExtractDirectory(false),
                                                    false,
                                                    true
                                                },
                                            null) as FileStream;
        }


        public static FileStream Create(string path, FileMode fileMode)
        {
            return Create(path, fileMode,
                          fileMode == FileMode.Append ? FileAccess.Write : FileAccess.Read | FileAccess.Write);
        }
    }
}