using System.IO;

namespace Utils.Extensions
{
    public static class StreamExtensions
    {
        public static void WriteToFile(this Stream stream, string filePath)
        {
            using (var fileStream = File.Create(filePath, (int)stream.Length))
            {
                // Initialize the bytes array with the stream length and then fill it with data.
                var bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, bytesInStream.Length);

                // Use write method to write to the file specified above.
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            }
        }
    }
}