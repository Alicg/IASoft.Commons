using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utils.Extensions
{
    public static class FileSystemExtension
    {
        #region Delegates

        public delegate string GetCustomVariableValueHandler(string key, string value);

        #endregion

        private const string LongPathPrefix = "\\\\?\\";

        /// <summary>
        /// Use for method ExpandPath - you can overload substituting %variable% values in input path 
        /// </summary>
        public static GetCustomVariableValueHandler GetCustomVariableValue;

        /// <summary>
        /// Gets or sets whether application directory is default directory for expanding relative pathes.
        /// If value is false default directory is current directory.
        /// </summary>
        public static bool DefaultBaseDirIsAppDir { get; set; }

        /// <summary>
        /// Returns current directory taking into account the value of property DefaultBaseDirIsAppDir.
        /// If current directory is empty application directory will be current one anyway.
        /// </summary>
        public static string GetCurrentDirectory(bool includePathSeparatorAtEnd = true)
        {
            string res = Directory.GetCurrentDirectory();
            if (DefaultBaseDirIsAppDir || res.IsEmpty(true))
                res = Assembly.GetEntryAssembly().Location.ExtractDirectory(false);
            return res + (includePathSeparatorAtEnd ? Path.DirectorySeparatorChar.ToString() : "");
        }

        /// <summary>
        /// Extracts directory from path
        /// </summary>
        public static string ExtractDirectory(this string path, bool includePathSeparatorAtEnd = true)
        {
            if (path.IsEmpty(true))
                return "";
            else
            {
                //var res = Path.GetDirectoryName(path);
                int idx = path.LastIndexOf(Path.DirectorySeparatorChar);
                if (idx == -1) return "";
                string res = path.Substring(0, idx);
                return res + (includePathSeparatorAtEnd ? Path.DirectorySeparatorChar.ToString() : "");
            }
        }

        /// <summary>
        /// Extracts file name from path (supports long file pathes > 260 characters)
        /// </summary>
        public static string ExtractFileName(this string path)
        {
            if (path.IsEmpty(true))
                return "";
            else
            {
                //var res = Path.GetFileName(path);
                int idx = path.LastIndexOf(Path.DirectorySeparatorChar);
                if (idx == -1) return path;
                string res = path.Substring(idx + 1);
                return res;
            }
        }

        /// <summary>
        /// Extracts file name without extension from path (supports long file pathes > 260 characters)
        /// </summary>
        public static string ExtractOnlyFileName(this string path)
        {
            if (path.IsEmpty(true))
                return "";
            else
            {
                return Path.GetFileNameWithoutExtension(path.ExtractFileName());
            }
        }

        /// <summary>
        /// Returns relative path from specified base directory.
        /// </summary>
        /// <param name="path">Source path</param>
        /// <param name="basePath">Base path</param>
        /// <returns>Relative path</returns>
        public static string GetRelativePath(this string path, string basePath = null)
        {
            try
            {
                if (basePath == null || basePath.IsEmpty(true))
                    basePath = GetCurrentDirectory();
                if (path.LeftPart(1) == @"\" && path.LeftPart(2) != @"\\")
                    path = Path.GetPathRoot(basePath.ExpandPath()) + path.Substring(1);
                if (path.IsEmpty()) return basePath;

                if (path.ToUpper().IndexOf(basePath.ToUpper()) != 0)
                    return path;
                path = path.Remove(0, Math.Min(basePath.Length + 1, path.Length));
                return path;
            }
            catch
            {
                return path;
            }
        }

        /// <summary>
        /// Expand relative path to full path from specified base path.<para></para>
        /// %AppDir% will be expanded from Apllication Exe Directory <para>
        /// %MyDocuments% will be expanded from User Documents Folder (Environment.SpecialFolder.MyDocuments) </para>
        /// %AppData% will be expanded from User application roaming data (Environment.SpecialFolder.ApplicationData - %UserProfile%\AppData\Roaming for Vista/Win7) <para>
        /// %CommonAppData% will be expanded from all users application roaming data (Environment.SpecialFolder.CommonApplicationData) </para>
        /// </summary>
        public static string ExpandPath(this string path, [Localizable(false)] string basePath = null)
        {
            string res = path;
            try
            {
                if (basePath == null || basePath.IsEmpty(true))
                    basePath = GetCurrentDirectory();
                else if (basePath == @"%AppDir%")
                    basePath = Assembly.GetEntryAssembly().Location.ExtractDirectory(false);
                Dictionary<string, string> variables = Environment
                    .GetEnvironmentVariables()
                    .Cast<DictionaryEntry>()
                    .ToDictionary(x => x.Key.AsString().ToUpper(), x => x.Value.AsString());
                variables["MyDocuments".ToUpper()] = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                variables["AppData".ToUpper()] = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                variables["CommonAppData".ToUpper()] =
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                variables["ProgramData".ToUpper()] = variables["CommonAppData".ToUpper()];
                variables["AppDir".ToUpper()] = Process.GetCurrentProcess().MainModule.FileName.ExtractDirectory(false);

                foreach (var variable in variables.ToArray())
                    variables[variable.Key] = OnGetCustomVariableValue(variable.Key, variable.Value);

                // Substitute environment variable values  
                if (path.IsNotEmpty())
                    path = Regex.Replace(path,
                                         @"(\%([a-z,A-Z,0-9]+)\%)",
                                         m =>
                                         m.Groups.Count >= 3
                                             ? variables.Get<string>(m.Groups[2].ToString().ToUpper())
                                             : m.ToString(),
                                         RegexOptions.IgnoreCase);
                if (basePath.IsNotEmpty())
                    basePath = Regex.Replace(basePath,
                                             @"(\%([a-z,A-Z,0-9]+)\%)",
                                             m =>
                                             m.Groups.Count >= 3
                                                 ? variables.Get<string>(m.Groups[2].ToString().ToUpper())
                                                 : m.ToString(),
                                             RegexOptions.IgnoreCase);

                string s = Directory.GetCurrentDirectory();
                try
                {
                    if (path.IsEmpty())
                        res = basePath;
                    else
                    {
                        try
                        {
                            // At first try standard method
                            Directory.SetCurrentDirectory(basePath);
                            res = Path.GetFullPath(path);
                        }
                        catch
                        {
                            // At first try standard method can be failed if file not exists - try manual
                            if (path.LeftPart(2) != @"\\")
                            {
                                if (path.LeftPart(1) == @"\")
                                    res = Path.GetPathRoot(basePath.ExpandPath()) + path.Substring(1);
                                else if (path.Length <= 2 || path[1] != ':')
                                    res = basePath + @"\" + path;
                            }
                        }
                    }
                }
                finally
                {
                    Directory.SetCurrentDirectory(s);
                }
            }
            catch
            {
            }
            return res;
        }

        private static string OnGetCustomVariableValue(string key, string value)
        {
            return GetCustomVariableValue != null ? GetCustomVariableValue(key, value) : value;
        }

        /// <summary>
        /// Delete directory and all files and subdirectories in it.
        /// </summary>
        public static void DeleteDirectory(this string path)
        {
            Directory.EnumerateFiles(path).ForEach(File.Delete);
            Directory.EnumerateDirectories(path).ForEach(d => d.DeleteDirectory());
            Directory.Delete(path);
        }

        /// <summary>
        /// Checks whether the path is network path.
        /// </summary>
        /// <param name="path">Checking path.</param>
        /// <returns>Returns true if path is network one.</returns>
        public static bool IsNetworkPath(this string path)
        {
            return path.LeftPart(2) == @"\\";
        }

        public static bool SetPermissions(this string path, [Localizable(false)] string identity,
                                          FileSystemRights rights, AccessControlType access)
        {
            try
            {
                if (File.Exists(path))
                {
                    var fileInfo = new FileInfo(path);
                    FileSecurity fileSecurity = fileInfo.GetAccessControl();
                    fileSecurity.AddAccessRule(new FileSystemAccessRule(identity, rights, access));
                    fileInfo.SetAccessControl(fileSecurity);
                }
                else if (Directory.Exists(path))
                {
                    var dirInfo = new DirectoryInfo(path);
                    DirectorySecurity dirSecurity = dirInfo.GetAccessControl();
                    dirSecurity.AddAccessRule(new FileSystemAccessRule(identity, rights, access));
                    dirInfo.SetAccessControl(dirSecurity);
                }
                else
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static string AddLongPathPrefix(string path)
        {
            if (path.StartsWith(LongPathPrefix, StringComparison.Ordinal))
                return path;
            else
                return LongPathPrefix + path;
        }


        /// <summary>
        /// Gets short name of existing file. If file doesn't exist returns empty string
        /// </summary>
        /// <param name="fileName">Long file name</param>
        public static string GetShortFileName(this string fileName)
        {
            var shortPath = new StringBuilder(255);
            WinApi.GetShortPathName(fileName, shortPath, shortPath.Capacity);
            return shortPath.ToString();
        }


        /// <summary>
        /// Creates directory if it doesn't exist. Returns short name of the directory and set access right if <paramref name="setAllUsersAccess"/> set to true.
        /// </summary>
        public static string CreateDirectory(string folder, bool setAllUsersAccess = true)
        {
            string res = folder.GetShortFileName();
            if (res.IsEmpty())
            {
                string parentFolder = folder.ExtractDirectory(false);
                CreateDirectory(parentFolder, setAllUsersAccess);
                WinApi.CreateDirectory(AddLongPathPrefix(folder), IntPtr.Zero);
                res = folder.GetShortFileName();
                if (res.IsEmpty())
                {
                    return folder;
                }
                if (setAllUsersAccess)
                    res.SetPermissions("USERS", FileSystemRights.FullControl, AccessControlType.Allow);
            }
            else if (setAllUsersAccess)
            {
                res.SetPermissions("USERS", FileSystemRights.FullControl, AccessControlType.Allow);
            }
            return res;
        }


        /// <summary>
        /// Read all file content into array of bytes. Supports long file pathes > 260 characters
        /// </summary>
        public static byte[] ReadAllBytes([Localizable(false)] string path)
        {
            byte[] buffer;
            using (FileStream fileStream = FileStreamCreator.Create(path, FileMode.Open, FileAccess.Read))
            {
                int offset = 0;
                long length = fileStream.Length;
                if (length > int.MaxValue)
                    throw new IOException("File too long.");
                var count = (int) length;
                buffer = new byte[count];
                while (count > 0)
                {
                    int num = fileStream.Read(buffer, offset, count);
                    if (num == 0)
                        break;
                    offset += num;
                    count -= num;
                }
            }
            return buffer;
        }    
  
        /// <summary>
        /// Read all file content into array of bytes. Supports long file pathes > 260 characters
        /// </summary>
        public static byte[] ReadAllBytes([Localizable(false)] FileStream fileStream)
        {
            byte[] buffer;
            int offset = 0;
            long length = fileStream.Length;
            if (length > int.MaxValue)
                throw new IOException("File too long.");
            var count = (int) length;
            buffer = new byte[count];
            while (count > 0)
            {
                int num = fileStream.Read(buffer, offset, count);
                if (num == 0)
                    break;
                offset += num;
                count -= num;
            }
            return buffer;
        }

        /// <summary>
        /// Check file name unique and/or file name length
        /// </summary>
        /// <param name="fileName">Source file name</param>
        /// <param name="autoCreateUnique">Create unique file name if file exists</param>
        /// <param name="maxFileNameLength">Limit file name length</param>
        /// <returns></returns>
        public static string CheckFileName(this string fileName, bool autoCreateUnique, int maxFileNameLength = -1)
        {
            string fDir = fileName.ExtractDirectory();
            string fName = fileName.ExtractOnlyFileName();
            if (maxFileNameLength > 0)
                fName = fName.Substring(0, maxFileNameLength.LimitMax(fName.Length));
            string fExt = Path.GetExtension(fileName);
            if (autoCreateUnique)
            {
                // Check whether the filename is already exists
                if (File.Exists(fileName))
                {
                    // Create unique file name
                    int i = 1;
                    do
                    {
                        if (maxFileNameLength > 0)
                        {
                            fName = fName.Substring(0, (maxFileNameLength - i.ToString().Length).LimitMax(fName.Length));
                            if (fileName == "")
                                return fileName;
                        }
                        fileName = Path.Combine(fDir, @"{0}{1}{2}".Fmt(fName, i++, fExt));
                    } while (File.Exists(fileName));
                }
            }
            else
            {
                fileName = Path.Combine(fDir, @"{0}{1}".Fmt(fName, fExt));
            }
            return fileName;
        }

        /// <summary>
        /// Check file name length
        /// </summary>
        /// <param name="fileName">Source file name</param>
        /// <param name="maxFileNameLength">Limit file name length</param>
        /// <returns></returns>
        public static string CheckFileName(this string fileName, int maxFileNameLength)
        {
            return fileName.CheckFileName(false, maxFileNameLength);
        }

        public static async Task<string> CreateFileSymbolicLink(string filePath)
        {
            var symbolicLink = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Path.GetExtension(filePath));

            System.Diagnostics.Process mklinkProcess = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C mklink \"{symbolicLink}\" \"{filePath}\"";
            mklinkProcess.StartInfo = startInfo;
            mklinkProcess.Start();
            
            if (mklinkProcess == null)
            {
                throw new Exception("Failed to create a symbolic link.");
            }
            await Task.Run(() => mklinkProcess.WaitForExit(5000));
            return symbolicLink;
        }
    }
}